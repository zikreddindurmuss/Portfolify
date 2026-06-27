"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card";
import { register, saveTokens } from "@/lib/api";
import { getErrorMessage } from "@/lib/utils";

const registerSchema = z
  .object({
    username: z
      .string()
      .min(3, "Kullanıcı adı en az 3 karakter olmalı")
      .max(30, "Kullanıcı adı en fazla 30 karakter olabilir")
      .regex(/^[a-z0-9_-]+$/, "Sadece küçük harf, rakam, _ ve - kullanabilirsiniz"),
    email: z.string().email("Geçerli bir e-posta adresi girin"),
    password: z
      .string()
      .min(8, "Şifre en az 8 karakter olmalı")
      .regex(/[A-Z]/, "En az bir büyük harf içermeli")
      .regex(/[0-9]/, "En az bir rakam içermeli"),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Şifreler eşleşmiyor",
    path: ["confirmPassword"],
  });

type RegisterFormValues = z.infer<typeof registerSchema>;

export function RegisterForm() {
  const router = useRouter();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register: formRegister,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
  });

  async function onSubmit(values: RegisterFormValues) {
    setServerError(null);
    try {
      const response = await register({
        username: values.username,
        email: values.email,
        password: values.password,
      });
      saveTokens(response.accessToken, response.refreshToken);
      router.push("/dashboard");
    } catch (err) {
      setServerError(getErrorMessage(err));
    }
  }

  return (
    <Card className="w-full max-w-md">
      <CardHeader className="text-center">
        <CardTitle className="text-2xl">Hesap Oluştur</CardTitle>
        <CardDescription>Portfolify&apos;a katıl, dijital kartvizitini yarat</CardDescription>
      </CardHeader>

      <CardContent>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {serverError && (
            <div className="rounded-md bg-destructive/10 border border-destructive/30 px-4 py-3 text-sm text-destructive">
              {serverError}
            </div>
          )}

          <div className="space-y-1.5">
            <Label htmlFor="username">Kullanıcı Adı</Label>
            <Input
              id="username"
              placeholder="ahmet_yilmaz"
              autoComplete="username"
              {...formRegister("username")}
            />
            {errors.username && (
              <p className="text-xs text-destructive">{errors.username.message}</p>
            )}
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="email">E-posta</Label>
            <Input
              id="email"
              type="email"
              placeholder="ahmet@example.com"
              autoComplete="email"
              {...formRegister("email")}
            />
            {errors.email && (
              <p className="text-xs text-destructive">{errors.email.message}</p>
            )}
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="password">Şifre</Label>
            <Input
              id="password"
              type="password"
              placeholder="••••••••"
              autoComplete="new-password"
              {...formRegister("password")}
            />
            {errors.password && (
              <p className="text-xs text-destructive">{errors.password.message}</p>
            )}
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="confirmPassword">Şifre Tekrar</Label>
            <Input
              id="confirmPassword"
              type="password"
              placeholder="••••••••"
              autoComplete="new-password"
              {...formRegister("confirmPassword")}
            />
            {errors.confirmPassword && (
              <p className="text-xs text-destructive">{errors.confirmPassword.message}</p>
            )}
          </div>

          <Button type="submit" className="w-full" disabled={isSubmitting}>
            {isSubmitting ? "Kayıt yapılıyor…" : "Kayıt Ol"}
          </Button>
        </form>
      </CardContent>

      <CardFooter className="justify-center text-sm text-muted-foreground">
        Zaten hesabın var mı?&nbsp;
        <a href="/login" className="text-primary hover:underline font-medium">
          Giriş Yap
        </a>
      </CardFooter>
    </Card>
  );
}
