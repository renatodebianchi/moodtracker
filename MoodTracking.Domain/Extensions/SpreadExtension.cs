using System.Reflection; 
namespace Domain.Extensions 
{ 
    public static class SpreadExtension 
    { 
        public static TTarget Spread<TSource, TTarget>(this TTarget target, TSource source) 
        { 
            foreach (PropertyInfo sourceProp in typeof(TSource).GetProperties()) 
            { 
                PropertyInfo? targetProp = typeof(TTarget).GetProperty(sourceProp.Name); 
                try { 
                    targetProp?.SetValue(target, sourceProp.GetValue(source)); 
                } catch (Exception e) { Console.WriteLine("Erro ao processar Spread: " + e.Message); } 
            } 
            return target; 
        } 
        public static TTarget SpreadNotNull<TSource, TTarget>(this TTarget target, TSource source) 
        { 
            foreach (PropertyInfo sourceProp in typeof(TSource).GetProperties()) 
            { 
                PropertyInfo? targetProp = typeof(TTarget).GetProperty(sourceProp.Name); 
                try { 
                    if (sourceProp.GetValue(source) == null) 
                        continue; 
                    if (sourceProp.PropertyType.Name.Equals("DateTime")) 
                        if (((DateTime)sourceProp.GetValue(source)) == (new DateTime())) 
                            continue; 
                    targetProp?.SetValue(target, sourceProp.GetValue(source)); 
                } catch (Exception e) { Console.WriteLine("Erro ao processar Spread: " + e.Message); } 
            } 
            return target; 
        } 
    } 
} 
