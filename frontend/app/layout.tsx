import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { ThemeProvider } from "@/components/theme-provider";
import { ThemeToggle } from "@/components/theme-toggle";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Portfolify — Dijital Kartvizit Platformu",
  description:
    "GitHub, LinkedIn ve blog adreslerini tek bir sayfada topla. Yazılımcılar için akıllı dijital kartvizit.",
};

// Hydration öncesi tema flaş'ını (FOUC) önlemek için senkron inline script.
const themeInitScript = `
(function () {
  try {
    var stored = window.localStorage.getItem("portfolify-theme");
    var systemPrefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
    var theme = stored || (systemPrefersDark ? "dark" : "light");
    if (theme === "dark") document.documentElement.classList.add("dark");
  } catch (e) {}
})();
`;

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="tr" suppressHydrationWarning>
      <head>
        <script dangerouslySetInnerHTML={{ __html: themeInitScript }} />
      </head>
      <body className={inter.className} suppressHydrationWarning>
        <ThemeProvider>
          <ThemeToggle />
          {children}
        </ThemeProvider>
      </body>
    </html>
  );
}
