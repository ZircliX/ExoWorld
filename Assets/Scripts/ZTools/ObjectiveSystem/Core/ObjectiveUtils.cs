namespace ZTools.ObjectiveSystem.Core
{
    public static class ObjectiveUtils
    {
        public static T GetHandlerByData<T>(this ObjectiveData objectiveData) where T : class, IObjectiveHandler
        {
            IObjectiveHandler handler = objectiveData.GetHandler();

            foreach (IObjectiveHandler obj in ObjectivesManager.ActiveObjectives)
            {
                //Debug.Log(obj.ObjectiveData.Name + " / " + obj.GetType());
                
                if (obj.GetType() == handler.GetType())
                    return obj as T;
            }

            throw new System.InvalidCastException($"Cannot cast handler of type {handler.GetType().Name} to type {typeof(T).Name}");
        }
    }
}