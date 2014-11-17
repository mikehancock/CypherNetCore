namespace CypherTwo.Core
{
    using Newtonsoft.Json.Linq;

    internal class Datum
    {
        public JToken[] row { get; set; }
    }

    internal class Result
    {
        public string[] columns { get; set; }

        public Datum[] data { get; set; }
    }

    internal class NeoResponse
    {
        public Result[] results { get; set; }

        public JObject[] errors { get; set; }
    }

    internal class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
