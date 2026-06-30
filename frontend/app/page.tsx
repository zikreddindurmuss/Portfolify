import Link from "next/link";
import { Button } from "@/components/ui/button";

export default function HomePage() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center bg-background px-4">
      {/* Hero */}
      <div className="max-w-3xl animate-fade-in text-center space-y-6">
        {/* Badge */}
        <span className="inline-block rounded-full border border-primary/40 bg-primary/10 px-4 py-1 text-xs font-semibold text-primary tracking-widest uppercase">
          Yazılımcılar için Dijital Kartvizit
        </span>

        <h1 className="text-5xl sm:text-6xl font-extrabold tracking-tight text-foreground">
          Portfoli<span className="text-primary">fy</span>
        </h1>

        <p className="text-lg sm:text-xl text-muted-foreground max-w-xl mx-auto leading-relaxed">
          GitHub, LinkedIn ve blog adreslerini tek bir sayfada topla.
          Yazılımcılar için akıllı dijital kartvizit platformu.
        </p>

        {/* CTA buttons */}
        <div className="flex flex-col sm:flex-row gap-3 justify-center pt-2">
          <Button asChild size="lg" className="text-base px-8">
            <Link href="/register">Ücretsiz Başla</Link>
          </Button>
          <Button asChild size="lg" variant="outline" className="text-base px-8">
            <Link href="/login">Giriş Yap</Link>
          </Button>
        </div>
      </div>

      {/* Feature grid */}
      <div className="mt-20 grid grid-cols-1 sm:grid-cols-3 gap-6 max-w-4xl w-full text-center">
        {[
          {
            icon: "🔗",
            title: "Tek Link",
            desc: "portfolify.app/kullaniciadiniz — tüm profillerini bir URL ile paylaş",
          },
          {
            icon: "🎨",
            title: "Özelleştirilebilir",
            desc: "Profil sayfanı kendi tarzına göre düzenle",
          },
          {
            icon: "📊",
            title: "Analitik",
            desc: "Profiline kaç kişinin baktığını gerçek zamanlı izle",
          },
        ].map((f) => (
          <div
            key={f.title}
            className="card-hover rounded-xl border border-border bg-card p-6 space-y-3"
          >
            <span className="text-3xl">{f.icon}</span>
            <h3 className="font-semibold text-foreground">{f.title}</h3>
            <p className="text-sm text-muted-foreground">{f.desc}</p>
          </div>
        ))}
      </div>

      {/* Footer */}
      <footer className="mt-20 text-xs text-muted-foreground">
        © {new Date().getFullYear()} Portfolify. Tüm hakları saklıdır.
      </footer>
    </main>
  );
}
