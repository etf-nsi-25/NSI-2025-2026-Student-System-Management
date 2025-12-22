namespace Analytics.Core.Entities
{
        public class Metric
        {
            public int Id { get; set; }          
            public string Name { get; set; } = null!; 
            public int Value { get; set; }            
            public int FacultyId { get; set; }        
            public string UserId { get; set; } = null!; 
            public DateTime Timestamp { get; set; }   
        }
    

}
