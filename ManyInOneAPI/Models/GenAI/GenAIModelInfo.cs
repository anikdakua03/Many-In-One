namespace ManyInOneAPI.Models.GenAI
{
    public class GenAIModelInfo : GenAIModelInfoMethod
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? InputTokenLimit { get; set; }
        public string? OutputTokenLimit { get; set; }
        public List<GenAIModelInfoMethod>? ModelGenerationMethods { get; set; }
        public string? Temperature { get; set; }
        public string? TopP { get; set; }
        public string? TopK { get; set; }
    }
}
