using DynamicTypeCreator;
using System.Reflection;
using System.Reflection.Emit;

public class Program
{
    private static void Main(string[] args)
    {
        // 创建一个动态类型构建器。
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DispatchProxy"),
            AssemblyBuilderAccess.Run);

        // 定义一个模块。
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("testmod");

        // 基类类型。
        var baseType = typeof(DispatchProxyBase);

        // 创建一个类型构建器。
        TypeBuilder typeBuilder = moduleBuilder.DefineType("DispatchProxyClass",
            TypeAttributes.Public, baseType);

        // 接口类型。
        var interfaceType = typeof(IMyInterface);

        // 添加接口实现。
        typeBuilder.AddInterfaceImplementation(interfaceType);

        // 获取基类的 Invoke 方法。
        var invokeMethod = baseType.GetMethod("Invoke");

        // 开始实现接口的方法，每个实现的方法都调用基类的 Invoke 方法。
        foreach (MethodInfo mi in interfaceType.GetRuntimeMethods())
        {
            if (!mi.IsVirtual || mi.IsFinal)
                continue;

            // 创建一个类型构建器。
            var methodBuilder = typeBuilder.DefineMethod(mi.Name,
                MethodAttributes.Public
                | MethodAttributes.Virtual
                | MethodAttributes.Final
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot,
                CallingConventions.Standard,
                mi.ReturnType, null, null,
                null, null, null);

            var generator = methodBuilder.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0); // 实例入栈。

            generator.Emit(OpCodes.Call, invokeMethod); // 调用 Invoke 方法。

            generator.Emit(OpCodes.Ret);
        }

        // 获取 DispatchProxyClass 类型。
        var dispatchProxyClass = typeBuilder.CreateType();

        var instance = Activator.CreateInstance(dispatchProxyClass) as IMyInterface;

        // 调用方法。
        instance.DoWork();
        instance.SaySomething();
    }
}