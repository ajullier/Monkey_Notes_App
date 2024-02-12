namespace NotesApi.Application
{
    public class GenericMessage<T>
    {
        public T? Object { get; set; }
        public string Message { get; set; } = string.Empty;
        public GenericMessage(T oObject, string message) {
            Object= oObject;
            Message = message;
        }
    }
}
