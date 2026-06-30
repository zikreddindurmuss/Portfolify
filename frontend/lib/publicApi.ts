import type { PublicProfile } from "@/types/profile";
import type { Skill } from "@/types/skill";

const BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

/**
 * Public profil sayfası (/[slug]) için ayrı, sade bir client.
 *
 * Bilinçli olarak `apiClient` (lib/api.ts) kullanılmıyor: o instance JWT
 * interceptor'larıyla donatılmış, ama bu sayfa tamamen public — token
 * eklemenin hiçbir anlamı yok. Düz `fetch` yeterli.
 */
export async function getPublicProfile(slug: string): Promise<PublicProfile | null> {
  const res = await fetch(`${BASE_URL}/api/profiles/${encodeURIComponent(slug)}`, {
    cache: "no-store",
  });

  if (res.status === 404) {
    return null;
  }

  if (!res.ok) {
    throw new Error(`Profil yüklenemedi (${res.status})`);
  }

  return (await res.json()) as PublicProfile;
}

/**
 * Public profil sayfasındaki skill listesi — token'sız.
 * Giriş yapmış bir ziyaretçi varsa endorse durumu `getUserSkills` (lib/api.ts)
 * üzerinden, JWT ile birlikte ayrıca sorgulanır.
 */
export async function getPublicUserSkills(slug: string): Promise<Skill[]> {
  const res = await fetch(`${BASE_URL}/api/skills/${encodeURIComponent(slug)}`, {
    cache: "no-store",
  });

  if (res.status === 404) {
    return [];
  }

  if (!res.ok) {
    throw new Error(`Skill listesi yüklenemedi (${res.status})`);
  }

  return (await res.json()) as Skill[];
}
