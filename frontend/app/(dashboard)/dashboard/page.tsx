"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { LayoutGrid, LogOut, Sparkles, UserCog } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { ProfileEditForm } from "@/components/profile/ProfileEditForm";
import { clearTokens, getCurrentUser, isAuthenticated, logout } from "@/lib/api";
import { cn } from "@/lib/utils";
import type { UserDto } from "@/types/auth";

type Section = "overview" | "edit" | "analytics";

const NAV_ITEMS: { id: Section; label: string; icon: typeof LayoutGrid }[] = [
  { id: "overview", label: "Genel Bakış", icon: LayoutGrid },
  { id: "edit", label: "Profili Düzenle", icon: UserCog },
  { id: "analytics", label: "Analitik", icon: Sparkles },
];

export default function DashboardPage() {
  const router = useRouter();
  const [user, setUser] = useState<UserDto | null>(null);
  const [activeSection, setActiveSection] = useState<Section>("overview");

  useEffect(() => {
    if (!isAuthenticated()) {
      router.replace("/login");
      return;
    }

    getCurrentUser()
      .then(setUser)
      .catch(() => {
        // Token geçersiz/kullanıcı silinmiş — oturumu temizleyip login'e dön.
        clearTokens();
        router.replace("/login");
      });
  }, [router]);

  async function handleLogout() {
    try {
      await logout();
    } catch {
      // Backend'e ulaşılamasa bile lokal oturumu temizleyip çıkış yapıyoruz.
    } finally {
      clearTokens();
      router.replace("/login");
    }
  }

  if (!user) return null;

  return (
    <div className="min-h-screen bg-background md:grid md:grid-cols-[16rem_1fr]">
      {/* Sidebar */}
      <aside className="flex flex-col gap-8 border-b border-border p-6 md:sticky md:top-0 md:h-screen md:border-b-0 md:border-r">
        <Link href="/" className="text-xl font-extrabold tracking-tight text-primary">
          Portfolify
        </Link>

        {/* Profil özeti */}
        <div className="flex items-center gap-3 rounded-xl border border-border bg-card p-4">
          <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-primary/15 text-lg font-bold text-primary">
            {user.username?.charAt(0).toUpperCase() ?? "?"}
          </div>
          <div className="min-w-0">
            <p className="truncate text-sm font-semibold text-foreground">{user.username}</p>
            <Link
              href={`/${user.slug}`}
              target="_blank"
              rel="noopener noreferrer"
              className="truncate text-xs text-primary hover:underline"
            >
              portfolify.app/{user.slug}
            </Link>
          </div>
        </div>

        {/* Navigasyon */}
        <nav className="flex flex-col gap-1">
          {NAV_ITEMS.map((item) => {
            const Icon = item.icon;
            const isDisabled = item.id === "analytics";
            const isActive = activeSection === item.id;
            return (
              <button
                key={item.id}
                type="button"
                onClick={() => setActiveSection(item.id)}
                className={cn(
                  "flex items-center gap-3 rounded-lg px-3 py-2.5 text-left text-sm font-medium transition-colors",
                  isActive
                    ? "bg-primary/15 text-primary"
                    : "text-muted-foreground hover:bg-accent hover:text-foreground",
                  isDisabled && !isActive && "opacity-70"
                )}
              >
                <Icon className="h-4 w-4 shrink-0" />
                {item.label}
                {isDisabled && (
                  <span className="ml-auto rounded-full bg-muted px-2 py-0.5 text-[10px] font-semibold text-muted-foreground">
                    Yakında
                  </span>
                )}
              </button>
            );
          })}
        </nav>

        <div className="mt-auto">
          <Button variant="outline" size="sm" className="w-full justify-start gap-2" onClick={handleLogout}>
            <LogOut className="h-4 w-4" />
            Çıkış Yap
          </Button>
        </div>
      </aside>

      {/* Ana içerik */}
      <main className="px-4 py-8 md:px-10 md:py-10">
        <div className="mx-auto max-w-3xl animate-fade-in space-y-8">
          {activeSection === "overview" && (
            <>
              <div>
                <h1 className="text-2xl font-bold text-foreground">Hoş Geldin, {user.username} 👋</h1>
                <p className="mt-1 text-sm text-muted-foreground">
                  Profilini düzenle ve dijital kartvizitini paylaş.
                </p>
              </div>

              <Card>
                <CardContent className="grid grid-cols-1 gap-4 pt-6 sm:grid-cols-3">
                  <Link
                    href={`/${user.slug}`}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="rounded-lg border border-border bg-muted/30 p-4 space-y-1 transition-colors hover:border-primary/50 hover:bg-muted/50"
                  >
                    <p className="text-xs text-muted-foreground">Profil URL&apos;in</p>
                    <p className="text-sm font-medium text-primary underline-offset-2 hover:underline">
                      portfolify.app/{user.slug}
                    </p>
                  </Link>

                  {[
                    { label: "Kullanıcı Adı", value: user.username ?? "—" },
                    { label: "Durum", value: "Aktif" },
                  ].map((item) => (
                    <div
                      key={item.label}
                      className="rounded-lg border border-border bg-muted/30 p-4 space-y-1"
                    >
                      <p className="text-xs text-muted-foreground">{item.label}</p>
                      <p className="text-sm font-medium text-foreground">{item.value}</p>
                    </div>
                  ))}
                </CardContent>
              </Card>

              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <button
                  type="button"
                  onClick={() => setActiveSection("edit")}
                  className="w-full text-left"
                >
                  <Card hoverable>
                    <CardHeader>
                      <CardTitle className="text-base">Profili Düzenle</CardTitle>
                      <CardDescription>GitHub, LinkedIn ve blog linklerini ekle</CardDescription>
                    </CardHeader>
                  </Card>
                </button>

                <button
                  type="button"
                  onClick={() => setActiveSection("analytics")}
                  className="w-full text-left"
                >
                  <Card hoverable className="opacity-80">
                    <CardHeader>
                      <CardTitle className="text-base">Analitik</CardTitle>
                      <CardDescription>Profiline gelen ziyaretleri incele</CardDescription>
                    </CardHeader>
                  </Card>
                </button>
              </div>
            </>
          )}

          {activeSection === "edit" && (
            <>
              <div>
                <h1 className="text-2xl font-bold text-foreground">Profili Düzenle</h1>
                <p className="mt-1 text-sm text-muted-foreground">
                  GitHub, LinkedIn ve blog linklerini güncelle, skill ekle.
                </p>
              </div>
              <ProfileEditForm onClose={() => setActiveSection("overview")} />
            </>
          )}

          {activeSection === "analytics" && (
            <>
              <div>
                <h1 className="text-2xl font-bold text-foreground">Analitik</h1>
                <p className="mt-1 text-sm text-muted-foreground">
                  Profiline gelen ziyaretleri yakında burada izleyebileceksin.
                </p>
              </div>
              <Card>
                <CardContent className="flex flex-col items-center gap-3 py-16 text-center">
                  <Sparkles className="h-8 w-8 text-primary" />
                  <p className="text-sm text-muted-foreground">Bu özellik yakında geliyor.</p>
                </CardContent>
              </Card>
            </>
          )}
        </div>
      </main>
    </div>
  );
}
