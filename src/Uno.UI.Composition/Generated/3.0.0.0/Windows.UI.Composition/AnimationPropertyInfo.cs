#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.UI.Composition
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class AnimationPropertyInfo : global::Windows.UI.Composition.CompositionObject
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Composition.AnimationPropertyAccessMode AccessMode
		{
			get
			{
				throw new global::System.NotImplementedException("The member AnimationPropertyAccessMode AnimationPropertyInfo.AccessMode is not implemented in Uno.");
			}
			set
			{
				global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.AnimationPropertyInfo", "AnimationPropertyAccessMode AnimationPropertyInfo.AccessMode");
			}
		}
		#endif
		// Forced skipping of method Windows.UI.Composition.AnimationPropertyInfo.AccessMode.get
		// Forced skipping of method Windows.UI.Composition.AnimationPropertyInfo.AccessMode.set
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Composition.CompositionObject GetResolvedCompositionObject()
		{
			throw new global::System.NotImplementedException("The member CompositionObject AnimationPropertyInfo.GetResolvedCompositionObject() is not implemented in Uno.");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  string GetResolvedCompositionObjectProperty()
		{
			throw new global::System.NotImplementedException("The member string AnimationPropertyInfo.GetResolvedCompositionObjectProperty() is not implemented in Uno.");
		}
		#endif
	}
}
