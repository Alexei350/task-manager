using System.Collections.Generic;
using TaskManager.Models.Enums;

namespace TaskManager.Models.Base
{    
    public class ReturnData
    {
        /// <summary>
        /// Indica se houve sucesso na requisição
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Lista de mensagens de retorno
        /// </summary>
        public List<ReturnMessage> Messages { get; init; }

        /// <summary>
        /// Retorna um objeto com uma mensagem de Informação
        /// </summary>
        /// <param name="message">Mensagem a ser retornada</param>
        /// <returns></returns>
        public static ReturnData ReturnInfo(string message)
        {
            return new ReturnData
            {
                Success = true,
                Messages = [
                    new ReturnMessage(ReturnMessageTypeEnum.Info, message)
                ]
            };
        }

        /// <summary>
        /// Retorna um objeto com uma mensagem de Sucesso
        /// </summary>
        /// <param name="message">Mensagem a ser retornada</param>
        /// <returns></returns>
        public static ReturnData ReturnSuccess(string message)
        {
            return new ReturnData
            {
                Success = true,
                Messages = [
                    new ReturnMessage(ReturnMessageTypeEnum.Success, message)
                ]
            };
        }

        /// <summary>
        /// Retorna um objeto com uma mensagem de Aviso
        /// </summary>
        /// <param name="message">Mensagem a ser retornada</param>
        /// <returns></returns>
        public static ReturnData ReturnWarning(string message)
        {
            return new ReturnData
            {
                Success = false,
                Messages = [
                    new ReturnMessage(ReturnMessageTypeEnum.Warning, message)
                ]
            };
        }

        /// <summary>
        /// Retorna um objeto com uma mensagem de Erro
        /// </summary>
        /// <param name="message">Mensagem a ser retornada</param>
        /// <returns></returns>
        public static ReturnData ReturnError(string message)
        {
            return new ReturnData
            {
                Success = false,
                Messages = [
                    new ReturnMessage(ReturnMessageTypeEnum.Error, message)
                ]
            };
        }
    }

    public class ReturnData<T> : ReturnData
    {
        public T Data { get; init; }

        /// <summary>
        /// Retorna um objeto com uma mensagem de Informação
        /// </summary>
        /// <param name="message">Mensagem a ser retornada</param>
        /// <returns></returns>
        public new static ReturnData<T> ReturnInfo(string message)
        {
            return new ReturnData<T>
            {
                Success = true,
                Messages = [
                    new ReturnMessage(ReturnMessageTypeEnum.Info, message)
                ]
            };
        }

        /// <summary>
        /// Retorna um objeto com uma mensagem de Sucesso
        /// </summary>
        /// <param name="message">Mensagem a ser retornada</param>
        /// <returns></returns>
        public new static ReturnData<T> ReturnSuccess(string message)
        {
            return new ReturnData<T>
            {
                Success = true,
                Messages = [
                    new ReturnMessage(ReturnMessageTypeEnum.Success, message)
                ]
            };
        }

        /// <summary>
        /// Retorna um objeto com uma mensagem de Aviso
        /// </summary>
        /// <param name="message">Mensagem a ser retornada</param>
        /// <returns></returns>
        public new static ReturnData<T> ReturnWarning(string message)
        {
            return new ReturnData<T>
            {
                Success = false,
                Messages = [
                    new ReturnMessage(ReturnMessageTypeEnum.Warning, message)
                ]
            };
        }

        /// <summary>
        /// Retorna um objeto com uma mensagem de Erro
        /// </summary>
        /// <param name="message">Mensagem a ser retornada</param>
        /// <returns></returns>
        public new static ReturnData<T> ReturnError(string message)
        {
            return new ReturnData<T>
            {
                Success = false,
                Messages = [
                    new ReturnMessage(ReturnMessageTypeEnum.Error, message)
                ]
            };
        }
    }
    
    public class ReturnMessage(
        ReturnMessageTypeEnum type,
        string message
    )
    {
        public ReturnMessageTypeEnum Type { get; private set; } = type;
        public string Message { get; private set; } = message;
    }
}