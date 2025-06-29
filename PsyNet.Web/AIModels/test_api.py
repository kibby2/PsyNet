import requests
import json

def test_ai_api():
    """Test the AI Medicine Recommendation API"""
    base_url = "http://localhost:5001"
    
    print("Testing AI Medicine Recommendation API...")
    print("="*50)
    
    # Test health check
    try:
        response = requests.get(f"{base_url}/health")
        if response.status_code == 200:
            health_data = response.json()
            print("✓ Health check passed")
            print(f"  Model loaded: {health_data.get('model_loaded', False)}")
            print(f"  Data loaded: {health_data.get('data_loaded', False)}")
        else:
            print("✗ Health check failed")
            return
    except Exception as e:
        print(f"✗ Health check error: {e}")
        return
    
    # Test get recommendations
    try:
        test_data = {
            "age": "35-44",
            "sex": "female",
            "condition": "depression",
            "patient_id": "test-123-456-789"
        }
        
        response = requests.post(
            f"{base_url}/recommend",
            json=test_data,
            headers={'Content-Type': 'application/json'}
        )
        
        if response.status_code == 200:
            recommendations = response.json()
            print("✓ Recommendations generated successfully")
            print(f"  Number of recommendations: {recommendations.get('count', 0)}")
            
            for i, rec in enumerate(recommendations.get('recommendations', []), 1):
                print(f"  {i}. {rec.get('medicineName', 'Unknown')} - Confidence: {rec.get('confidenceScore', 0):.2f}")
        else:
            print(f"✗ Recommendations failed: {response.status_code}")
            print(f"  Response: {response.text}")
    except Exception as e:
        print(f"✗ Recommendations error: {e}")
    
    # Test available conditions
    try:
        response = requests.get(f"{base_url}/available-conditions")
        if response.status_code == 200:
            conditions = response.json()
            print("✓ Available conditions retrieved")
            print(f"  Number of conditions: {len(conditions.get('conditions', []))}")
        else:
            print("✗ Failed to get conditions")
    except Exception as e:
        print(f"✗ Conditions error: {e}")
    
    # Test available age groups
    try:
        response = requests.get(f"{base_url}/available-age-groups")
        if response.status_code == 200:
            age_groups = response.json()
            print("✓ Available age groups retrieved")
            print(f"  Number of age groups: {len(age_groups.get('age_groups', []))}")
        else:
            print("✗ Failed to get age groups")
    except Exception as e:
        print(f"✗ Age groups error: {e}")
    
    print("="*50)
    print("API test completed!")

if __name__ == "__main__":
    test_ai_api()
