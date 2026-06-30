"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { clearTokens, isAuthenticated, logout } from "@/lib/api";
import type { UserDto } from "@/types/auth";

export default function DashboardPage() {
  const router = useRouter();
  const [user, setUser] = useState<Partial<UserDto> | null>(null);

  useEffect(() => {
    if (!isAuthenticated()) {
      router.replace("/login");
      return;
    }
    // TODO: fetch current user from /api/users/me when endpoint is ready
    // For now, show a placeholder
    setUser({ username: "Kullanıcı", slug: "slug" });
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
    <main className="min-h-screen bg-background px-4 py-8">
      <div className="max-w-4xl mx-auto space-y-8">
        {/* Header */}
        <header className="flex items-center justify-between">
          <span className="text-2xl font-extrabold text-primary">Portfolify</span>
          <Button variant="outline" size="sm" onClick={handleLogout}>
            Çıkış Yap
          </Button>
        </header>

        {/* Welcome */}
        <Card>
          <CardHeader>
            <CardTitle>Hoş Geldin 👋</CardTitle>
            <CardDescription>
              Profilini düzenle ve dijital kartvizitini paylaş.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
              {[
                { label: "Profil URL'in", value: `portfolify.app/${user.slug ?? "—"}` },
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
            </div>

            <div className="pt-2 text-sm text-muted-foreground">
              Backend endpoint&apos;leri hazır olduğunda profil düzenleme ve analitik burada görünecek.
            </div>
          </CardContent>
        </Card>

        {/* Quick links */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {[
            { title: "Profili Düzenle", desc: "GitHub, LinkedIn ve blog linklerini ekle", disabled: true },
            { title: "Analitik", desc: "Profiline gelen ziyaretleri incele", disabled: true },
          ].map((item) => (
            <Card key={item.title} className="opacity-60">
              <CardHeader>
                <CardTitle className="text-base">{item.title}</CardTitle>
                <CardDescription>{item.desc}</CardDescription>
              </CardHeader>
            </Card>
          ))}
        </div>
      </div>
    </main>
  );
}
