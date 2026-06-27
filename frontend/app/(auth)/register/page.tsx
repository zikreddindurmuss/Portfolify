import { RegisterForm } from "@/components/auth/RegisterForm";

export const metadata = {
  title: "Kayıt Ol — Portfolify",
};

export default function RegisterPage() {
  return (
    <main className="min-h-screen flex items-center justify-center bg-background px-4">
      <div className="w-full max-w-md space-y-6">
        <div className="text-center">
          <a href="/" className="text-2xl font-extrabold text-primary">
            Portfolify
          </a>
        </div>
        <RegisterForm />
      </div>
    </main>
  );
}
