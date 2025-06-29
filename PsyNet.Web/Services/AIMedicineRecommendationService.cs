using PsyNet.Web.Models.Domain;
using System.Text.Json;

namespace PsyNet.Web.Services
{
    public class AIMedicineRecommendationService : IMedicineRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIMedicineRecommendationService> _logger;
        private const string AI_API_BASE_URL = "http://localhost:5001";

        public AIMedicineRecommendationService(HttpClient httpClient, ILogger<AIMedicineRecommendationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<MedicineRecommendation>> GetRecommendationsAsync(string age, string sex, string condition, Guid patientId)
        {
            try
            {
                var requestData = new
                {
                    age = age,
                    sex = sex,
                    condition = condition,
                    patient_id = patientId.ToString()
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation($"Calling AI API for recommendations: Age={age}, Sex={sex}, Condition={condition}");

                var response = await _httpClient.PostAsync($"{AI_API_BASE_URL}/recommend", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<AIRecommendationResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.Success == true && apiResponse.Recommendations != null)
                    {
                        var recommendations = new List<MedicineRecommendation>();

                        foreach (var rec in apiResponse.Recommendations)
                        {
                            recommendations.Add(new MedicineRecommendation
                            {
                                Id = Guid.NewGuid(),
                                PatientId = patientId,
                                MedicineName = rec.MedicineName,
                                Dosage = rec.Dosage,
                                Frequency = rec.Frequency,
                                Instructions = rec.Instructions,
                                ConfidenceScore = rec.ConfidenceScore,
                                RecommendationSource = rec.RecommendationSource,
                                IsActive = rec.IsActive,
                                CreatedAt = DateTime.UtcNow
                            });
                        }

                        _logger.LogInformation($"Successfully generated {recommendations.Count} AI recommendations");
                        return recommendations;
                    }
                    else
                    {
                        _logger.LogWarning("AI API returned unsuccessful response or no recommendations");
                        return GetFallbackRecommendations(age, sex, condition, patientId);
                    }
                }
                else
                {
                    _logger.LogError($"AI API request failed with status: {response.StatusCode}");
                    return GetFallbackRecommendations(age, sex, condition, patientId);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error calling AI API");
                return GetFallbackRecommendations(age, sex, condition, patientId);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "AI API request timed out");
                return GetFallbackRecommendations(age, sex, condition, patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling AI API");
                return GetFallbackRecommendations(age, sex, condition, patientId);
            }
        }

        public async Task<List<string>> GetAvailableAgeGroupsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{AI_API_BASE_URL}/available-age-groups");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<AgeGroupsResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return apiResponse?.AgeGroups ?? GetDefaultAgeGroups();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching age groups from AI API");
            }

            return GetDefaultAgeGroups();
        }

        public async Task<List<string>> GetAvailableConditionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{AI_API_BASE_URL}/available-conditions");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ConditionsResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return apiResponse?.Conditions ?? GetDefaultConditions();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching conditions from AI API");
            }

            return GetDefaultConditions();
        }

        // Legacy synchronous methods for backward compatibility
        public List<string> GetAvailableAgeGroups()
        {
            return GetAvailableAgeGroupsAsync().GetAwaiter().GetResult();
        }

        public List<string> GetAvailableConditions()
        {
            return GetAvailableConditionsAsync().GetAwaiter().GetResult();
        }

        private List<MedicineRecommendation> GetFallbackRecommendations(string age, string sex, string condition, Guid patientId)
        {
            _logger.LogInformation("Using fallback recommendation system");

            var fallbackRecommendations = new List<MedicineRecommendation>();

            // Basic fallback recommendations based on common psychiatric conditions
            var commonRecommendations = condition.ToLower() switch
            {
                var c when c.Contains("depression") => new[]
                {
                    ("Sertraline", "50mg", "Once daily", "SSRI antidepressant - take with food"),
                    ("Fluoxetine", "20mg", "Once daily", "SSRI antidepressant - morning dose recommended"),
                    ("Escitalopram", "10mg", "Once daily", "SSRI antidepressant - can be taken with or without food")
                },
                var c when c.Contains("anxiety") => new[]
                {
                    ("Lorazepam", "0.5mg", "As needed", "Short-term anxiety relief - use with caution"),
                    ("Buspirone", "15mg", "Twice daily", "Non-addictive anxiety medication"),
                    ("Sertraline", "25mg", "Once daily", "SSRI for anxiety disorders")
                },
                var c when c.Contains("bipolar") => new[]
                {
                    ("Lithium", "300mg", "Twice daily", "Mood stabilizer - requires blood monitoring"),
                    ("Quetiapine", "25mg", "Once daily", "Atypical antipsychotic for mood stabilization"),
                    ("Valproate", "250mg", "Twice daily", "Anticonvulsant used as mood stabilizer")
                },
                _ => new[]
                {
                    ("Consult Psychiatrist", "N/A", "As needed", "Please consult with a mental health professional"),
                    ("Therapy Referral", "N/A", "Weekly", "Consider psychotherapy as first-line treatment"),
                    ("Lifestyle Changes", "N/A", "Daily", "Exercise, sleep hygiene, and stress management")
                }
            };

            for (int i = 0; i < commonRecommendations.Length; i++)
            {
                var (medicine, dosage, frequency, instructions) = commonRecommendations[i];

                fallbackRecommendations.Add(new MedicineRecommendation
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    MedicineName = medicine,
                    Dosage = dosage,
                    Frequency = frequency,
                    Instructions = instructions,
                    ConfidenceScore = 0.5, // Lower confidence for fallback
                    RecommendationSource = "Fallback_System",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return fallbackRecommendations;
        }

        private List<string> GetDefaultAgeGroups()
        {
            return new List<string>
            {
                "18-24", "25-34", "35-44", "45-54", "55-64", "65-74", "75+"
            };
        }

        private List<string> GetDefaultConditions()
        {
            return new List<string>
            {
                "Depression", "Anxiety", "Bipolar Disorder", "PTSD", "OCD",
                "Schizophrenia", "ADHD", "Panic Disorder", "GAD", "Social Anxiety"
            };
        }
    }

    // Response models for AI API
    public class AIRecommendationResponse
    {
        public bool Success { get; set; }
        public List<AIRecommendation> Recommendations { get; set; } = new();
        public int Count { get; set; }
    }

    public class AIRecommendation
    {
        public string Id { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public string RecommendationSource { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class AgeGroupsResponse
    {
        public bool Success { get; set; }
        public List<string> AgeGroups { get; set; } = new();
    }

    public class ConditionsResponse
    {
        public bool Success { get; set; }
        public List<string> Conditions { get; set; } = new();
    }
}
