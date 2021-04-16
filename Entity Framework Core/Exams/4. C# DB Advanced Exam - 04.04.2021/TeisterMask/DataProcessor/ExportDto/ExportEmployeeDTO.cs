namespace TeisterMask.DataProcessor.ExportDto
{
    public class ExportEmployeeDTO
    {
        public string Username { get; set; }

        public ExportEmployeeTaskDTO[] Tasks { get; set; }
    }
}
