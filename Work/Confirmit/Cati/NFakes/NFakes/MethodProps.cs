namespace NFakes
{
    public class MethodProps
    {
        public string InterfaceName { get; set; }
        public string MethodName { get; set; }
        public string ReturnType { get; set; }
        public string Parameters { get; set; }
        public string Arguments { get; set; }
        public string DelegateName { get; set; }
        public string DelegateType { get; set; }
        public string OutParametersInitialisation { get; set; }
        public bool IsVoid { get; set; }
        public string GenericArguments { get; set; }
        public bool IsGeneric { get; set; }
    }
}