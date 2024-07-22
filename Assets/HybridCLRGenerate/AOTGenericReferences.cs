using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"DOTween.dll",
		"Shared-Lib.dll",
		"UniRx.dll",
		"UnityEngine.CoreModule.dll",
		"Zenject.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// SukharevShared.EnumResolver.<>c<int,object>
	// SukharevShared.EnumResolver<int,object>
	// System.Action<long>
	// System.Action<object>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<object>
	// System.Comparison<object>
	// System.Func<object,byte>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IObservable<long>
	// System.Predicate<object>
	// UniRx.Observer.Subscribe<long>
	// UniRx.Observer.Subscribe_<long>
	// UnityEngine.Events.InvokableCall<UnityEngine.Vector3>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.InvokableCall<object>
	// UnityEngine.Events.UnityAction<UnityEngine.Vector3>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<UnityEngine.Vector3>
	// UnityEngine.Events.UnityEvent<float>
	// UnityEngine.Events.UnityEvent<object>
	// Zenject.ConcreteBinderGeneric.<>c__DisplayClass5_0<object>
	// Zenject.ConcreteBinderGeneric<object>
	// Zenject.ConcreteIdBinderGeneric<object>
	// Zenject.FromBinderGeneric.<>c__DisplayClass14_0<object>
	// Zenject.FromBinderGeneric.<>c__DisplayClass15_0<object>
	// Zenject.FromBinderGeneric.<>c__DisplayClass3_0<object>
	// Zenject.FromBinderGeneric<object>
	// }}

	public void RefMethods()
	{
		// object DG.Tweening.TweenSettingsExtensions.OnComplete<object>(object,DG.Tweening.TweenCallback)
		// bool ModestTree.TypeExtensions.DerivesFrom<object>(System.Type)
		// System.IDisposable UniRx.ObservableExtensions.Subscribe<long>(System.IObservable<long>,System.Action<long>)
		// System.IObserver<long> UniRx.Observer.CreateSubscribeObserver<long>(System.Action<long>,System.Action<System.Exception>,System.Action)
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// Zenject.ConcreteIdBinderGeneric<object> Zenject.DiContainer.Bind<object>()
		// Zenject.ConcreteIdBinderGeneric<object> Zenject.DiContainer.Bind<object>(Zenject.BindStatement)
	}
}