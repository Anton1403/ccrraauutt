namespace crraut.Models; 

public class TelegramMessageDto {
    public string Title { get; set; }
    public bool IsExistOnMexc { get; set; }
    public bool IsExistOnGateIo { get; set; }
    public string Url { get; set; }
    public DateTime Date { get; set; }

    public bool IsTokenNameNotFound { get; set; } = false;
}