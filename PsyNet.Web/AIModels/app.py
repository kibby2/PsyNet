from flask import Flask, request, jsonify
import pandas as pd
import numpy as np
import joblib
from sklearn.metrics.pairwise import cosine_similarity
from textblob import TextBlob
import logging
import os
from flask_cors import CORS

app = Flask(__name__)
CORS(app)  # Enable CORS for cross-origin requests from .NET app

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Global variables to store model and data
svm_model = None
data = None
user_profiles = None
drugs = None

def load_model_and_data():
    """Load the SVM model and preprocessed data"""
    global svm_model, data, user_profiles, drugs
    
    try:
        # Load the SVM model
        svm_model = joblib.load('svm_model.pkl')
        logger.info("SVM model loaded successfully")
        
        # Load and preprocess the data
        data = pd.read_csv('psychiatric_drug.csv')
        data = data.dropna()
        
        # Drop unnecessary columns
        data.drop(columns=[col for col in ['Unnamed: 0', 'date', 'time_on_drug', 'reviewer_type', 
                          'rating_effectiveness', 'rating_ease_of_use', 'rating_satisfaction'] 
                          if col in data.columns], inplace=True)
        
        # Add sentiment analysis
        def get_sentiment(review):
            analysis = TextBlob(str(review))
            if analysis.sentiment.polarity > 0:
                return 'positive'
            elif analysis.sentiment.polarity == 0:
                return 'neutral'
            else:
                return 'negative'
        
        data['sentiment'] = data['text'].apply(get_sentiment)
        
        # Create user profiles for collaborative filtering
        user_profiles = pd.get_dummies(data[['age', 'gender', 'condition']])
        drugs = data['drug_name']
        
        logger.info(f"Data loaded successfully. {len(data)} records processed")
        
    except Exception as e:
        logger.error(f"Error loading model and data: {str(e)}")
        raise

def recommend_drugs_for_user(age, sex, condition, top_n=5):
    """
    Generate drug recommendations using collaborative filtering and sentiment analysis
    """
    try:
        # Map gender to match data format
        gender_map = {'male': 'Male', 'female': 'Female', 'Male': 'Male', 'Female': 'Female'}
        mapped_gender = gender_map.get(sex.lower(), sex)
        
        # Create a profile for the input user
        user_data = pd.DataFrame({
            'age': [age],
            'gender': [mapped_gender],
            'condition': [condition]
        })
        
        user_profile_encoded = pd.get_dummies(user_data)
        
        # Ensure the new user profile has the same columns as existing profiles
        user_profile_encoded = user_profile_encoded.reindex(columns=user_profiles.columns, fill_value=0)
        
        # Calculate similarity between input user and existing users
        similarity_scores = cosine_similarity(user_profile_encoded, user_profiles)[0]
        
        # Get indices of most similar users
        similar_users_indices = np.argsort(similarity_scores)[::-1]
        
        # Use SVM model to predict sentiment for similar users
        similar_users_data = data.iloc[similar_users_indices[:100]]  # Top 100 similar users
        
        # Predict sentiment using SVM model (if available)
        try:
            sentiment_predictions = svm_model.predict(user_profiles.iloc[similar_users_indices[:100]])
        except:
            # Fallback to existing sentiment if SVM prediction fails
            sentiment_predictions = similar_users_data['sentiment'].values
        
        # Generate recommendations based on positive sentiment
        recommended_drugs = []
        confidence_scores = []
        unique_drugs = set()
        
        for i, (user_idx, sentiment) in enumerate(zip(similar_users_indices[:100], sentiment_predictions)):
            if sentiment == 'positive' or sentiment == 1:  # SVM might return 1 for positive
                drug_name = drugs.iloc[user_idx]
                similarity_score = similarity_scores[user_idx]
                
                if drug_name not in unique_drugs and similarity_score > 0:
                    recommended_drugs.append({
                        'medicine_name': drug_name,
                        'confidence_score': float(similarity_score),
                        'recommendation_source': 'AI_ML_Model',
                        'dosage': 'As prescribed by physician',
                        'frequency': 'As prescribed by physician',
                        'instructions': f'Recommended based on similar patient profiles with positive outcomes for {condition}'
                    })
                    unique_drugs.add(drug_name)
                    
                    if len(recommended_drugs) >= top_n:
                        break
        
        # If we don't have enough recommendations, add some popular drugs for the condition
        if len(recommended_drugs) < top_n:
            condition_drugs = data[data['condition'].str.contains(condition, case=False, na=False)]
            popular_drugs = condition_drugs.groupby('drug_name').size().sort_values(ascending=False)
            
            for drug_name, count in popular_drugs.head(top_n - len(recommended_drugs)).items():
                if drug_name not in unique_drugs:
                    recommended_drugs.append({
                        'medicine_name': drug_name,
                        'confidence_score': 0.5,  # Lower confidence for popular drugs
                        'recommendation_source': 'Popular_for_Condition',
                        'dosage': 'As prescribed by physician',
                        'frequency': 'As prescribed by physician',
                        'instructions': f'Commonly prescribed for {condition}'
                    })
        
        return recommended_drugs
        
    except Exception as e:
        logger.error(f"Error generating recommendations: {str(e)}")
        return []

@app.route('/health', methods=['GET'])
def health_check():
    """Health check endpoint"""
    return jsonify({
        'status': 'healthy',
        'model_loaded': svm_model is not None,
        'data_loaded': data is not None
    })

@app.route('/recommend', methods=['POST'])
def get_recommendations():
    """
    Get medicine recommendations for a patient
    Expected JSON payload:
    {
        "age": "45-54",
        "sex": "male",
        "condition": "depression",
        "patient_id": "guid-string"
    }
    """
    try:
        request_data = request.get_json()
        
        if not request_data:
            return jsonify({'error': 'No JSON data provided'}), 400
        
        age = request_data.get('age')
        sex = request_data.get('sex')
        condition = request_data.get('condition')
        patient_id = request_data.get('patient_id')
        
        if not all([age, sex, condition, patient_id]):
            return jsonify({'error': 'Missing required parameters: age, sex, condition, patient_id'}), 400
        
        # Generate recommendations
        recommendations = recommend_drugs_for_user(age, sex, condition)
        
        # Format response for .NET application
        formatted_recommendations = []
        for i, rec in enumerate(recommendations):
            formatted_recommendations.append({
                'id': f"{patient_id}-{i}",
                'patientId': patient_id,
                'medicineName': rec['medicine_name'],
                'dosage': rec['dosage'],
                'frequency': rec['frequency'],
                'instructions': rec['instructions'],
                'confidenceScore': rec['confidence_score'],
                'recommendationSource': rec['recommendation_source'],
                'isActive': True,
                'createdAt': pd.Timestamp.now().isoformat()
            })
        
        return jsonify({
            'success': True,
            'recommendations': formatted_recommendations,
            'count': len(formatted_recommendations)
        })
        
    except Exception as e:
        logger.error(f"Error in /recommend endpoint: {str(e)}")
        return jsonify({'error': f'Internal server error: {str(e)}'}), 500

@app.route('/available-conditions', methods=['GET'])
def get_available_conditions():
    """Get list of available medical conditions"""
    try:
        if data is not None:
            conditions = sorted(data['condition'].unique().tolist())
            return jsonify({
                'success': True,
                'conditions': conditions
            })
        else:
            return jsonify({'error': 'Data not loaded'}), 500
    except Exception as e:
        logger.error(f"Error getting conditions: {str(e)}")
        return jsonify({'error': str(e)}), 500

@app.route('/available-age-groups', methods=['GET'])
def get_available_age_groups():
    """Get list of available age groups"""
    try:
        if data is not None:
            age_groups = sorted(data['age'].unique().tolist())
            return jsonify({
                'success': True,
                'age_groups': age_groups
            })
        else:
            return jsonify({'error': 'Data not loaded'}), 500
    except Exception as e:
        logger.error(f"Error getting age groups: {str(e)}")
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    # Load model and data on startup
    load_model_and_data()
    
    # Run the Flask app
    app.run(host='localhost', port=5001, debug=True)
