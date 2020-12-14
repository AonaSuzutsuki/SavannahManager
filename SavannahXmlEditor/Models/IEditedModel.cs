namespace _7dtd_XmlEditor.Models
{
    public interface IEditedModel
    {
        bool IsEdited { get; set; }
        string FullPath { get; set; }
    }
}