namespace AdBoardAPI.Options
{
    public class SystemOptions
    {
        [Exists(ErrorMessage = "Указанная директория статических файлов не существует")]
        public string StaticFilesRoot { get; set; }
    }
}
