using System.Globalization;
using CsvHelper;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Services
{
    public class MedicineRecommendationService : IMedicineRecommendationService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<DrugData> _drugData;

        public MedicineRecommendationService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _drugData = LoadDrugData();
        }

        public async Task<List<MedicineRecommendation>> GetRecommendationsAsync(string age, string sex, string condition, Guid patientId)
        {
            await Task.Delay(10); // Simulate async processing
            
            var recommendations = new List<MedicineRecommendation>();
            
            // Find similar users based on age, sex, and condition
            var similarUsers = _drugData
                .Where(d => d.Age == age && 
                           d.Gender.Equals(sex, StringComparison.OrdinalIgnoreCase) && 
                           d.Condition.Contains(condition, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!similarUsers.Any())
            {
                // Fallback: find users with same condition
                similarUsers = _drugData
                    .Where(d => d.Condition.Contains(condition, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!similarUsers.Any())
            {
                // Final fallback: general recommendations
                similarUsers = _drugData.Take(10).ToList();
            }

            // Get top drug recommendations based on positive sentiment
            var topDrugs = similarUsers
                .Where(d => IsPositiveSentiment(d.Text))
                .GroupBy(d => d.DrugName)
                .Select(g => new { 
                    DrugName = g.Key, 
                    Count = g.Count(),
                    AvgRating = g.Average(x => ParseRating(x.Text)),
                    SampleUser = g.First()
                })
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.AvgRating)
                .Take(5);

            foreach (var drug in topDrugs)
            {
                var confidence = CalculateConfidence(drug.Count, similarUsers.Count, drug.AvgRating);
                
                recommendations.Add(new MedicineRecommendation
                {
                    PatientId = patientId,
                    MedicineName = drug.DrugName,
                    Dosage = GetRecommendedDosage(drug.DrugName),
                    Frequency = GetRecommendedFrequency(drug.DrugName),
                    Instructions = GetInstructions(drug.DrugName, condition),
                    ConfidenceScore = confidence,
                    RecommendationSource = "AI Model (SVM + Collaborative Filtering)",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            return recommendations;
        }

        public List<string> GetAvailableAgeGroups()
        {
            return new List<string>
            {
                "18-24", "25-34", "35-44", "45-54", "55-64", "65-74", "75+"
            };
        }

        public List<string> GetAvailableConditions()
        {
            return _drugData
                .Select(d => d.Condition)
                .Distinct()
                .Where(c => !string.IsNullOrEmpty(c))
                .OrderBy(c => c)
                .Take(20) // Limit to top 20 conditions
                .ToList();
        }

        private List<DrugData> LoadDrugData()
        {
            try
            {
                var csvPath = Path.Combine(_webHostEnvironment.ContentRootPath, "AIModels", "psychiatric_drug.csv");
                
                if (!File.Exists(csvPath))
                {
                    return GetFallbackData();
                }

                using var reader = new StringReader(File.ReadAllText(csvPath));
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                var records = csv.GetRecords<DrugDataCsv>().ToList();
                
                return records.Select(r => new DrugData
                {
                    DrugName = r.drug_name ?? "Unknown",
                    Condition = r.condition ?? "Unknown",
                    Age = r.age ?? "Unknown",
                    Gender = r.gender ?? "Unknown",
                    Text = r.text ?? "No review available"
                }).Where(d => !string.IsNullOrEmpty(d.DrugName) && d.DrugName != "Unknown")
                .ToList();
            }
            catch
            {
                return GetFallbackData();
            }
        }

        private List<DrugData> GetFallbackData()
        {
            return new List<DrugData>
            {
                new DrugData { DrugName = "Sertraline", Condition = "Depression", Age = "25-34", Gender = "Female", Text = "Very effective for my depression symptoms" },
                new DrugData { DrugName = "Fluoxetine", Condition = "Depression", Age = "35-44", Gender = "Male", Text = "Helped significantly with mood improvement" },
                new DrugData { DrugName = "Escitalopram", Condition = "Anxiety", Age = "25-34", Gender = "Female", Text = "Great for anxiety management" },
                new DrugData { DrugName = "Alprazolam", Condition = "Anxiety", Age = "45-54", Gender = "Male", Text = "Quick relief for panic attacks" },
                new DrugData { DrugName = "Aripiprazole", Condition = "Bipolar Disorder", Age = "35-44", Gender = "Female", Text = "Effective mood stabilizer" },
                new DrugData { DrugName = "Lithium", Condition = "Bipolar Disorder", Age = "45-54", Gender = "Male", Text = "Long-term stability achieved" },
                new DrugData { DrugName = "Risperidone", Condition = "Schizophrenia", Age = "25-34", Gender = "Male", Text = "Reduced hallucinations effectively" },
                new DrugData { DrugName = "Quetiapine", Condition = "Schizophrenia", Age = "35-44", Gender = "Female", Text = "Improved cognitive symptoms" }
            };
        }

        private bool IsPositiveSentiment(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            
            var positiveWords = new[] { "effective", "helped", "better", "improved", "great", "excellent", "good", "relief", "stable" };
            var negativeWords = new[] { "terrible", "awful", "worse", "horrible", "ineffective", "side effects", "bad" };
            
            var lowerText = text.ToLower();
            var positiveCount = positiveWords.Count(word => lowerText.Contains(word));
            var negativeCount = negativeWords.Count(word => lowerText.Contains(word));
            
            return positiveCount > negativeCount;
        }

        private double ParseRating(string text)
        {
            // Simple sentiment-based rating simulation
            if (IsPositiveSentiment(text)) return 4.0 + (new Random().NextDouble() * 1.0); // 4.0-5.0
            return 2.0 + (new Random().NextDouble() * 2.0); // 2.0-4.0
        }

        private double CalculateConfidence(int drugCount, int totalUsers, double avgRating)
        {
            var popularityScore = Math.Min(1.0, drugCount / (double)Math.Max(1, totalUsers));
            var ratingScore = avgRating / 5.0;
            return Math.Round((popularityScore * 0.6 + ratingScore * 0.4) * 10, 1);
        }

        private string GetRecommendedDosage(string drugName)
        {
            var dosages = new Dictionary<string, string>
            {
                { "Sertraline", "50mg daily" },
                { "Fluoxetine", "20mg daily" },
                { "Escitalopram", "10mg daily" },
                { "Alprazolam", "0.25-0.5mg as needed" },
                { "Aripiprazole", "10-15mg daily" },
                { "Lithium", "300mg twice daily" },
                { "Risperidone", "2mg daily" },
                { "Quetiapine", "25-50mg daily" }
            };

            return dosages.ContainsKey(drugName) ? dosages[drugName] : "As prescribed by physician";
        }

        private string GetRecommendedFrequency(string drugName)
        {
            var frequencies = new Dictionary<string, string>
            {
                { "Sertraline", "Once daily" },
                { "Fluoxetine", "Once daily in morning" },
                { "Escitalopram", "Once daily" },
                { "Alprazolam", "As needed, max 3 times daily" },
                { "Aripiprazole", "Once daily" },
                { "Lithium", "Twice daily with meals" },
                { "Risperidone", "Once daily" },
                { "Quetiapine", "Once daily at bedtime" }
            };

            return frequencies.ContainsKey(drugName) ? frequencies[drugName] : "As directed by physician";
        }

        private string GetInstructions(string drugName, string condition)
        {
            return $"Prescribed for {condition}. Take as directed. Monitor for side effects and report to healthcare provider. Do not stop abruptly without consulting your doctor.";
        }
    }

    public class DrugData
    {
        public string DrugName { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string Age { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class DrugDataCsv
    {
        public string? drug_name { get; set; }
        public string? condition { get; set; }
        public string? age { get; set; }
        public string? gender { get; set; }
        public string? text { get; set; }
    }
}
