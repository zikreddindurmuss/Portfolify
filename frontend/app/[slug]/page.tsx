"use client";

import { use, useEffect, useState } from "react";
import { Github, Linkedin, Link as LinkIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { getPublicProfile, getPublicUserSkills } from "@/lib/publicApi";
import { endorseSkill, getCurrentUser, getUserSkills, isAuthenticated, removeEndorsement } from "@/lib/api";
import { getErrorMessage } from "@/lib/utils";
import type { PublicProfile } from "@/types/profile";
import type { Skill } from "@/types/skill";
import type { UserDto } from "@/types/auth";

const LINK_ICONS: Record<string, typeof Github> = {
  GitHub: Github,
  LinkedIn: Linkedin,
  Blog: LinkIcon,
};

interface PublicProfilePageProps {
  params: Promise<{ slug: string }>;
}

export default function PublicProfilePage({ params }: PublicProfilePageProps) {
  const { slug } = use(params);

  const [profile, setProfile] = useState<PublicProfile | null>(null);
  const [profileNotFound, setProfileNotFound] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const [skills, setSkills] = useState<Skill[]>([]);
  const [skillError, setSkillError] = useState<string | null>(null);
  const [pendingSkillId, setPendingSkillId] = useState<string | null>(null);

  const [currentUser, setCurrentUser] = useState<UserDto | null>(null);

  useEffect(() => {
    let cancelled = false;

    getPublicProfile(slug)
      .then((data) => {
        if (cancelled) return;
        if (!data) {
          setProfileNotFound(true);
        } else {
          setProfile(data);
        }
      })
      .catch(() => {
        if (!cancelled) setProfileNotFound(true);
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false);
      });

    const loggedIn = isAuthenticated();

    (loggedIn ? getUserSkills(slug) : getPublicUserSkills(slug))
      .then((data) => {
        if (!cancelled) setSkills(data);
      })
      .catch((err) => {
        if (!cancelled) setSkillError(getErrorMessage(err));
      });

    if (loggedIn) {
      getCurrentUser()
        .then((user) => {
          if (!cancelled) setCurrentUser(user);
        })
        .catch(() => {
          // Token geçersizse sessizce anonim ziyaretçi gibi davran.
        });
    }

    return () => {
      cancelled = true;
    };
  }, [slug]);

  async function handleToggleEndorse(skill: Skill) {
    setSkillError(null);
    setPendingSkillId(skill.id);
    try {
      if (skill.isEndorsedByMe) {
        await removeEndorsement(skill.id);
      } else {
        await endorseSkill(skill.id);
      }
      setSkills((prev) =>
        prev.map((s) =>
          s.id === skill.id
            ? {
                ...s,
                isEndorsedByMe: !s.isEndorsedByMe,
                endorsementCount: s.endorsementCount + (s.isEndorsedByMe ? -1 : 1),
              }
            : s
        )
      );
    } catch (err) {
      setSkillError(getErrorMessage(err));
    } finally {
      setPendingSkillId(null);
    }
  }

  if (isLoading) {
    return null;
  }

  if (profileNotFound || !profile) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-background px-4 py-12">
        <Card className="w-full max-w-md animate-fade-in">
          <CardHeader className="text-center">
            <CardTitle>Profil bulunamadı</CardTitle>
            <CardDescription>Bu kullanıcı mevcut değil.</CardDescription>
          </CardHeader>
        </Card>
      </main>
    );
  }

  const links = [
    { label: "GitHub", url: profile.githubUrl },
    { label: "LinkedIn", url: profile.linkedinUrl },
    { label: "Blog", url: profile.blogUrl },
  ].filter((link): link is { label: string; url: string } => !!link.url);

  const isOwnProfile = currentUser?.slug === profile.slug;

  return (
    <main className="flex min-h-screen items-center justify-center bg-background px-4 py-12">
      <Card className="w-full max-w-2xl animate-fade-in">
        <CardHeader className="text-center">
          <div className="mx-auto mb-2 flex h-16 w-16 items-center justify-center rounded-full bg-primary/15 text-2xl font-bold text-primary">
            {profile.username?.charAt(0).toUpperCase()}
          </div>
          <CardTitle className="text-2xl">{profile.username}</CardTitle>
          {profile.bio && <CardDescription className="text-base">{profile.bio}</CardDescription>}
        </CardHeader>

        {links.length > 0 && (
          <CardContent className="flex flex-col gap-3 sm:flex-row sm:justify-center">
            {links.map((link) => {
              const Icon = LINK_ICONS[link.label] ?? LinkIcon;
              return (
                <Button key={link.label} asChild variant="outline" className="gap-2">
                  <a href={link.url} target="_blank" rel="noopener noreferrer">
                    <Icon className="h-4 w-4" />
                    {link.label}
                  </a>
                </Button>
              );
            })}
          </CardContent>
        )}

        {skills.length > 0 && (
          <CardContent className="space-y-3 border-t border-border pt-6">
            <p className="text-sm font-medium text-foreground">Yetenekler</p>

            {skillError && (
              <div className="rounded-md bg-destructive/10 border border-destructive/30 px-4 py-3 text-sm text-destructive">
                {skillError}
              </div>
            )}

            <ul className="flex flex-wrap gap-2">
              {skills.map((skill) => (
                <li key={skill.id}>
                  <Badge variant={skill.isEndorsedByMe ? "endorsed" : "neutral"}>
                    <span>{skill.name}</span>
                    <span className="text-xs opacity-70">{skill.endorsementCount}</span>
                    {currentUser && !isOwnProfile && (
                      <button
                        type="button"
                        onClick={() => handleToggleEndorse(skill)}
                        disabled={pendingSkillId === skill.id}
                        className="text-xs font-medium underline-offset-2 hover:underline disabled:opacity-50"
                      >
                        {skill.isEndorsedByMe ? "Geri Al" : "Endorse"}
                      </button>
                    )}
                  </Badge>
                </li>
              ))}
            </ul>
          </CardContent>
        )}
      </Card>
    </main>
  );
}
