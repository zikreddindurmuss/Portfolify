"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import {
  addSkill,
  getCurrentUser,
  getMyProfile,
  getUserSkills,
  removeSkill,
  updateMyProfile,
} from "@/lib/api";
import { getErrorMessage } from "@/lib/utils";
import type { Skill } from "@/types/skill";

// Boş string'e izin ver (alan opsiyonel), doluysa geçerli bir URL olmalı.
const optionalUrl = z
  .string()
  .trim()
  .refine((value) => value === "" || z.string().url().safeParse(value).success, {
    message: "Geçerli bir URL girin (https://... ile başlamalı)",
  });

const profileSchema = z.object({
  bio: z.string().trim().max(500, "Bio en fazla 500 karakter olabilir"),
  githubUrl: optionalUrl,
  linkedinUrl: optionalUrl,
  blogUrl: optionalUrl,
});

type ProfileFormValues = z.infer<typeof profileSchema>;

interface ProfileEditFormProps {
  onClose?: () => void;
}

export function ProfileEditForm({ onClose }: ProfileEditFormProps) {
  const [isLoading, setIsLoading] = useState(true);
  const [serverError, setServerError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const [skills, setSkills] = useState<Skill[]>([]);
  const [newSkillName, setNewSkillName] = useState("");
  const [skillError, setSkillError] = useState<string | null>(null);
  const [isAddingSkill, setIsAddingSkill] = useState(false);
  const [removingSkillId, setRemovingSkillId] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<ProfileFormValues>({
    resolver: zodResolver(profileSchema),
    defaultValues: { bio: "", githubUrl: "", linkedinUrl: "", blogUrl: "" },
  });

  useEffect(() => {
    let cancelled = false;

    getMyProfile()
      .then((profile) => {
        if (cancelled) return;
        reset({
          bio: profile.bio ?? "",
          githubUrl: profile.githubUrl ?? "",
          linkedinUrl: profile.linkedinUrl ?? "",
          blogUrl: profile.blogUrl ?? "",
        });
      })
      .catch((err) => {
        if (!cancelled) setServerError(getErrorMessage(err));
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false);
      });

    getCurrentUser()
      .then((user) => getUserSkills(user.slug))
      .then((data) => {
        if (!cancelled) setSkills(data);
      })
      .catch((err) => {
        if (!cancelled) setSkillError(getErrorMessage(err));
      });

    return () => {
      cancelled = true;
    };
  }, [reset]);

  async function handleAddSkill() {
    const name = newSkillName.trim();
    setSkillError(null);

    if (!name) {
      setSkillError("Skill adı boş olamaz.");
      return;
    }
    if (name.length > 50) {
      setSkillError("Skill adı en fazla 50 karakter olabilir.");
      return;
    }
    if (skills.some((s) => s.name.toLowerCase() === name.toLowerCase())) {
      setSkillError("Bu skill zaten eklenmiş.");
      return;
    }

    setIsAddingSkill(true);
    try {
      const skill = await addSkill(name);
      setSkills((prev) => [...prev, skill]);
      setNewSkillName("");
    } catch (err) {
      setSkillError(getErrorMessage(err));
    } finally {
      setIsAddingSkill(false);
    }
  }

  async function handleRemoveSkill(skillId: string) {
    setSkillError(null);
    setRemovingSkillId(skillId);
    try {
      await removeSkill(skillId);
      setSkills((prev) => prev.filter((s) => s.id !== skillId));
    } catch (err) {
      setSkillError(getErrorMessage(err));
    } finally {
      setRemovingSkillId(null);
    }
  }

  async function onSubmit(values: ProfileFormValues) {
    setServerError(null);
    setSuccessMessage(null);
    try {
      await updateMyProfile({
        bio: values.bio?.trim() || null,
        githubUrl: values.githubUrl?.trim() || null,
        linkedinUrl: values.linkedinUrl?.trim() || null,
        blogUrl: values.blogUrl?.trim() || null,
      });
      setSuccessMessage("Kaydedildi.");
    } catch (err) {
      setServerError(getErrorMessage(err));
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-lg">Profili Düzenle</CardTitle>
        <CardDescription>GitHub, LinkedIn ve blog linklerini ekle.</CardDescription>
      </CardHeader>

      <CardContent>
        {isLoading ? (
          <p className="text-sm text-muted-foreground">Yükleniyor…</p>
        ) : (
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            {serverError && (
              <div className="rounded-md bg-destructive/10 border border-destructive/30 px-4 py-3 text-sm text-destructive">
                {serverError}
              </div>
            )}

            {successMessage && (
              <div className="rounded-md bg-green-500/10 border border-green-500/30 px-4 py-3 text-sm text-green-600 dark:text-green-400">
                {successMessage}
              </div>
            )}

            <div className="space-y-1.5">
              <Label htmlFor="bio">Bio</Label>
              <Textarea
                id="bio"
                placeholder="Kendinden kısaca bahset…"
                {...register("bio")}
              />
              {errors.bio && (
                <p className="text-xs text-destructive">{errors.bio.message}</p>
              )}
            </div>

            <div className="space-y-1.5">
              <Label htmlFor="githubUrl">GitHub URL</Label>
              <Input
                id="githubUrl"
                placeholder="https://github.com/kullanici-adi"
                {...register("githubUrl")}
              />
              {errors.githubUrl && (
                <p className="text-xs text-destructive">{errors.githubUrl.message}</p>
              )}
            </div>

            <div className="space-y-1.5">
              <Label htmlFor="linkedinUrl">LinkedIn URL</Label>
              <Input
                id="linkedinUrl"
                placeholder="https://linkedin.com/in/kullanici-adi"
                {...register("linkedinUrl")}
              />
              {errors.linkedinUrl && (
                <p className="text-xs text-destructive">{errors.linkedinUrl.message}</p>
              )}
            </div>

            <div className="space-y-1.5">
              <Label htmlFor="blogUrl">Blog URL</Label>
              <Input
                id="blogUrl"
                placeholder="https://blogun.com"
                {...register("blogUrl")}
              />
              {errors.blogUrl && (
                <p className="text-xs text-destructive">{errors.blogUrl.message}</p>
              )}
            </div>

            <div className="flex items-center gap-3 pt-2">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Kaydediliyor…" : "Kaydet"}
              </Button>
              {onClose && (
                <Button type="button" variant="outline" onClick={onClose}>
                  Kapat
                </Button>
              )}
            </div>
          </form>
        )}

        {!isLoading && (
          <div className="mt-6 space-y-3 border-t border-border pt-6">
            <div>
              <p className="text-sm font-medium text-foreground">Yetenekler</p>
              <p className="text-xs text-muted-foreground">
                Profilinde gösterilecek skill'leri ekle veya kaldır.
              </p>
            </div>

            {skillError && (
              <div className="rounded-md bg-destructive/10 border border-destructive/30 px-4 py-3 text-sm text-destructive">
                {skillError}
              </div>
            )}

            {skills.length > 0 && (
              <ul className="flex flex-wrap gap-2">
                {skills.map((skill) => (
                  <li key={skill.id}>
                    <Badge variant="neutral">
                      <span>{skill.name}</span>
                      <span className="text-xs opacity-70">{skill.endorsementCount}</span>
                      <button
                        type="button"
                        onClick={() => handleRemoveSkill(skill.id)}
                        disabled={removingSkillId === skill.id}
                        className="text-muted-foreground hover:text-destructive disabled:opacity-50"
                        aria-label={`${skill.name} skill'ini sil`}
                      >
                        ×
                      </button>
                    </Badge>
                  </li>
                ))}
              </ul>
            )}

            <div className="flex items-center gap-2">
              <Input
                value={newSkillName}
                onChange={(e) => setNewSkillName(e.target.value)}
                placeholder="Yeni skill (ör. TypeScript)"
                maxLength={50}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    handleAddSkill();
                  }
                }}
              />
              <Button
                type="button"
                variant="outline"
                onClick={handleAddSkill}
                disabled={isAddingSkill}
              >
                {isAddingSkill ? "Ekleniyor…" : "Ekle"}
              </Button>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
