Modify from  adoconnection`s RazorEngineCore link:
https://github.com/adoconnection/RazorEngineCore/tree/master/RazorEngineCore
Add support for ViewBag, Layout and Partial View


### 代码修改
1. 代码注释中所标注`CustomEdit:`的地方都代表我们进行了扩展或者修改。
2. 每处修改会在冒号后面注明对应的原因。
3. 如果为代码段  那么会以`CustomEdit-Begin:`开始，以`CustomEdit-End`结束。


### 重要！！！后台代码中相关的类型安全方法被砍掉的原因

1. 原项目中关于类型安全类`RazorEngineTemplateBase`是基于`new`重写`Model`进行实现的，这样就导致重写的属性在本类内可以正常访问，但是通过父接口却访问到的是原本的虚方法。请参考`RazorEngineCompiledTemplate.cs`文件中以下的代码，在下面的方法中,`instance`对象是接口`IRazorEngineTemplate`，一旦我们具有`Layout`布局视图，我们却无法通过`instance.Model`将`Model`对象传到布局视图中。
    ``` c#
           T instance = (T)Activator.CreateInstance(this.templateType);
            .....
            //获取instance.Layout必须在 instance.Execute();之后否则无法取值
            if (!string.IsNullOrEmpty(instance.Layout))
            {
                //递归调用RenderRawT<T>方法渲染Layout视图
                //注意将渲染子视图参数设置为true
                return razor.RenderRawT<ModelT>(instance.Layout, (ModelT)instance.Model, instance.ViewBag, true);
            }
    ```

2. 针对编译完类型安全的`RazorEngineCompiledTemplateT`类，如果采用`Json`序列化存储，那么我们在反序列化时就需要指定对应的泛型`T`,所以针对`ICompiledViewProvider`接口，我们可能就需要提供以下这些接口，这增加了复杂度。
    ``` c#
     IRazorEngineCompiledTemplate Get(string relativePath);

    Task<IRazorEngineCompiledTemplate> GetAsync(string relativePath);

    IRazorEngineCompiledTemplate<T> Get<T>(string relativePath);

    Task<IRazorEngineCompiledTemplate<T>> GetAsync<T>(string relativePath);

    ...Set同理
    ```

3. 在对项目进行比较大的改动实现了类型安全的渲染方法后，使用相同内容的`View`进行测试，类型安全的渲染方法总是要比原来弱类型的渲染方法慢<span style="color:red">**10ms左右**</span>。

4. 综合以上几点，所以决定在后台代码中砍掉关于类型安全方法的实现，仅保留`RazorEngineTemplateBaseT`类在前端实现`Model`内容的提示，在后端依旧使用原本弱类型的方法进行渲染。


### 其他常见修改原因

#### 异步代码阻塞调用
项目中有很多阻塞异步执行的代码，例如下面这样：

``` c#
WriteLiteralAsync(literal).GetAwaiter().GetResult();
```
微软文档中原文是这样说的："`ASP.NET Core` 应用中的一个常见性能问题是阻塞可以异步进行的调用。 许多同步阻塞调用都会导致线程池饥饿和响应时间降低。
请勿通过调用 `Task.Wait` 或 `Task<TResult>.Result `来阻止异步执行"。

所以我们尽量将此类代码改掉，改为其原本的同步实现，而不是通过阻塞异步方法的方式。

参考链接:https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/best-practices?view=aspnetcore-6.0

#### 将同步api包装成异步

在`RazorEngine`中存在如下代码：

``` c#
public Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) where T : IRazorEngineTemplate
 {
     return Task.Factory.StartNew(() => this.Compile<T>(content: content, builderAction: builderAction));
 }
```

此方法在原本同步方法的基础上，使用`Task.Factory.StartNew()`包裹了一层，使之成为"异步方法",使用`Task.Run()`亦同理。微软文档中有关于此的描述：
1. `ASP.NET Core` 已经在普通线程池线程上运行应用代码，因此调用 `Task.Run` 只会导致不必要的额外线程池计划。
2. 请勿使用 `Task.Run` 使同步 `API` 异步。


这看起来是个错误或者说不太合理的做法，在翻阅了资料后大致得出结论，如果此类情况无法避免，
使用`Task.FromResult`或者`Task.CompletedTask`通常比`Task.Run`更好，因为前两个并不会额外启动一个`Task`，只是把返回结果包裹在一个`Task`中，所以我们将
上述代码改为使用`Task.FromResult`实现。

参考链接：https://stackoverflow.com/questions/34005397/task-fromresult-vs-task-run

