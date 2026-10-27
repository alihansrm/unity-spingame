# Pipeline Android build düzeltmesi

24 Ekim 2026 tarihinde Unity 6000.3.23f1 ve com.unity.pipeline 0.9.0-exp.1 ile
Android Build And Run, oyun derlenmeden şu hatayla duruyordu:

`ArgumentException: Subtarget property is not available for the platform (Android)`

Kaynak: `PipelineRuntimeBuildProcessor.OnPreprocessBuild` içinde Android BuildSummary
üzerinde `GetSubtarget<StandaloneBuildSubtarget>()` çağrılması.

Paket, Unity Package Manager `Client.Embed` ile proje içine alındı. Böylece düzeltme
Library/PackageCache silindiğinde de korunur; diğer proje kopyaları etkilenmez.

Düzenlenen dosya:
`Packages/com.unity.pipeline/Editor/BuildProcessors/PipelineRuntimeBuildProcessor.cs`

`GetSubtarget` artık yalnızca BuildTargetGroup.Standalone için çağrılır. Android ve
diğer platformlarda TargetFor'a Player varsayılanı verilir; TargetFor bu parametreyi
Standalone dışındaki gruplarda zaten kullanmaz.

Oyun kodları, input, ödül sistemi ve Android build ayarları bu düzeltmede değiştirilmedi.

Paketi ileride yeni bir resmî sürüme geçirirken bu yerel düzeltmenin yeni sürümde
bulunup bulunmadığını kontrol et. Embedded paket `Packages/com.unity.pipeline`
klasöründedir ve projeyle birlikte saklanmalıdır.

Doğrulama: Android IL2CPP ARMv7 + ARM64 APK derlemesi başarıyla tamamlandı; 0 hata, SpinDetector içindeki mevcut NoFilter() kullanımıyla ilgili 1 eskime uyarısı. Çıktı Builds/Android/SpinDemo.apk. Bu doğrulama telefona kurulum veya gerçek cihazda oyun testi içermez.
