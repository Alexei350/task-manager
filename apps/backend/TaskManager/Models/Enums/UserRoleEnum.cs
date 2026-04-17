using System.ComponentModel;

namespace TaskManager.Models.Enums
{
    public enum UserRoleEnum
    {
        [Description("Padrão")]
        Default = 0,
        
        [Description("Administrador")]
        Admin = 99,
    }
}