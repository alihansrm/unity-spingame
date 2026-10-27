# Telefona APK alma

Proje: **12-FinaleDemo4** — Unity **6000.3.23f1**.
24 Ekim 2026: Pipeline Android uyumluluk düzeltmesinden sonra APK başarıyla oluşturuldu (0 hata, 1 mevcut eskime uyarısı). Çıktı: Builds/Android/SpinDemo.apk.

## Hazır ayarlar

- Android hedef platformu; tek APK. AAB ve Export Project kapalı.
- Paket kimliği: `com.ali.spindemo`.
- Uygulama adı: `12-FinaleDemo4`; sürüm 1.0.0, Version Code 1.
- IL2CPP; ARM64 + ARMv7 aynı APK içinde.
- Minimum API 25 (Android 7.1), Target API 36 (kurulu SDK).
- Development Build, Script Debugging, Autoconnect Profiler kapalı.
- Tek başlangıç sahnesi: `Assets/Scenes/FinaleDemo.unity`.
- Yalnızca iki yatay yön; Input System dokunmatik UI bağlantıları etkin.
- Custom Keystore kapalı: yerel test APK'sı Unity'nin varsayılan debug anahtarıyla imzalanır.
- Çıktı yolu: `Builds/Android/SpinDemo.apk` (Assets klasörünün dışında).
- Atlaslar mevcut Editor prebuild yordamıyla build sırasında otomatik hazırlanır.

SDK, NDK, OpenJDK ve Gradle yolları bu bilgisayardaki Unity kurulumuna bağlı ve
gerekli araç dosyaları mevcut. İlk build sırasında Gradle bağımlılık indirmeleri
için internet gerekebilir. Ayar kontrolü, başarılı bir APK derlemesinin yerine geçmez.

## USB ile Build And Run

1. Telefonda Ayarlar > Telefon hakkında altında Yapım numarası / Build number
   alanına yedi kez dokun. Bazı markalarda bu alan Yazılım bilgileri altındadır.
2. Geliştirici seçeneklerinden **USB hata ayıklama** seçeneğini aç.
3. Telefonu veri aktarabilen USB kablosuyla bilgisayara bağla. Telefon kilidini aç;
   bu bilgisayar için USB hata ayıklama iznini onayla.
4. Unity Play modunda olmasın. **File > Build Profiles** penceresini aç.
5. **Android** platformunu seç. Zaten etkin; Switch Platform/Profile görünürse
   Android'e geç. Scene List'te `FinaleDemo` açık olmalı. Build App Bundle ve
   Export Project kapalı kalsın.
6. **Run Device** listesinden telefonunu seç; görünmüyorsa **Refresh** yap.
7. **Build And Run** seç. Dosya konumu sorulursa projenin `Builds/Android`
   klasörüne `SpinDemo.apk` adıyla kaydet.
8. Derleme tamamlandığında Unity APK'yı telefona kurup oyunu açar. İlk IL2CPP
   build'i daha uzun sürebilir; aynı anda ikinci build başlatma.

## USB olmadan, APK dosyasıyla kurulum

1. Aynı Build Profiles penceresinde **Build** seç; APK'yı yukarıdaki konuma kaydet.
2. Oluşan `SpinDemo.apk` dosyasını telefonuna aktar.
3. Telefonda dosyayı aç. Android isterse, APK'yı açtığın dosya yöneticisi veya
   tarayıcı için **bu kaynaktan uygulama yükleme** iznini ver ve kurulumu onayla.
4. Uygulama listesinden `12-FinaleDemo4` oyununu aç.

Bu ikinci yöntemde USB hata ayıklama gerekli değildir.

## Telefon görünmüyorsa

- Telefon kilidi açık olsun ve hata ayıklama izin penceresini onayla.
- Kablonun yalnızca şarj değil veri aktarımını da desteklediğini kontrol et.
- Gerekirse USB bağlantı modunu Dosya aktarımı olarak seç ve Refresh yap.
- Windows'ta bazı telefonlar üreticinin USB sürücüsünü gerektirir.

## Telefonda ilk kontrol

Başla > Spin > ödül > Devam; aynı ödülün miktar artışı; Çıkış > özet > ana menü;
bomba > kazançların silinmesi > ana menü; yeni oyunun sıfırlanması; iki yatay
yön, ekran kesikleri ve arka plan müziği kontrol edilmeli.

## Resmî kaynaklar

- [Unity Android build işlemi](https://docs.unity.com/en-us/engine/6000.0/manual/platform-specific/android/building-and-delivering/build-process)
- [Unity USB ile cihaz bağlantısı](https://docs.unity.com/en-us/engine/6000.0/manual/platform-specific/android/developing/testing-and-debugging/debugging-on-an-android-device)
- [Android geliştirici seçenekleri](https://developer.android.com/studio/debug/dev-options)
