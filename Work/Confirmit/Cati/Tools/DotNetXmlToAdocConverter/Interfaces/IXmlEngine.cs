using System.Collections.Generic;

namespace DotNetXmlToAdocConverter.Interfaces
{
    public interface IXmlEngine
    {
        List<ClassInfo> GetClasses();
    }
}