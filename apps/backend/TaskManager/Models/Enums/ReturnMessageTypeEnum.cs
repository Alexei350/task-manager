using System.ComponentModel;

namespace TaskManager.Models.Enums
{
    public enum ReturnMessageTypeEnum
    {
        [Description("Informação")]
        Info,

        [Description("Sucesso")]
        Success,

        [Description("Aviso")]
        Warning,

        [Description("Erro")]
        Error,
    }
}