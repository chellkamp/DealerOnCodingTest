namespace WebApp.Models
{
    public class WorkResult
    {
        /// <summary>
        /// Label for result
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Result to display to user
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Description of result
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public WorkResult()
        {
            Name = string.Empty;
            Value = string.Empty;
            Description = string.Empty;
        }
    }
}
