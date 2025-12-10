namespace ZTools.ObjectiveSystem.Core
{
    public static class ObjectiveUtils
    {
        public static T GetHandlerByData<T>(this ObjectiveData objectiveData) where T : IObjectiveHandler
        {
            IObjectiveHandler handler = objectiveData.GetHandler();

            foreach (IObjectiveHandler obj in ObjectivesManager.ActiveObjectives)
            {
                if (obj.GetType() == typeof(T))
                {
                    return (T)obj;
                }
            }

            throw new System.InvalidCastException($"Cannot cast handler of type {handler.GetType().Name} to type {typeof(T).Name}");
        }
    }
}