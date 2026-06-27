import { LoginForm } from "@/components/auth/LoginForm";

export const metadata = {
  title: "Giriş Yap — Portfolify",
};

export default function LoginPage() {
  return (
    <main className="min-h-screen flex items-center justify-center bg-background px-4">
      <div className="w-full max-w-md space-y-6">
        <div className="text-center">
          <a href="/" className="text-2xl font-extrabold text-primary">
            Portfolify
          </a>
        </div>
        <LoginForm />
      </div>
    </main>
  );
}
