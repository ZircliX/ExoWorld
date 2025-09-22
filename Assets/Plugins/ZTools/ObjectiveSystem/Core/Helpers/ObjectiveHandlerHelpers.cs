using ZTools.ObjectiveSystem.Core.Data;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace ZTools.ObjectiveSystem.Core.Helpers
{
    public static class ObjectiveHandlerHelpers
    {
        public static bool CastHandler<T>(this IObjectiveHandler handler, out T typedHandler) where T : class, IObjectiveHandler
        {
            if (handler is T h)
            {
                typedHandler = h;
                return true;
            }
            
            typedHandler = null;
            return false;
        }
        
        public static bool CastData<T>(this IObjectiveHandler handler, out T typedData) where T : ObjectiveData
        {
            if (handler.ObjectiveData is T d)
            {
                typedData = d;
                return true;
            }
            
            typedData = null;
            return false;
        }

        public static bool CastHandlerAndData<THandler, TData>(this IObjectiveHandler handler, 
            out THandler typedHandler,
            out TData typedData)
            where THandler : class, IObjectiveHandler
            where TData : ObjectiveData
        {
            bool hasHandler = handler.CastHandler(out typedHandler);
            bool hasData = handler.CastData(out typedData);
            return hasHandler && hasData;
        }
    }
}