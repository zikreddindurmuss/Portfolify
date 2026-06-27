import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Portfolify — Dijital Kartvizit Platformu",
  description:
    "GitHub, LinkedIn ve blog adreslerini tek bir sayfada topla. Yazılımcılar için akıllı dijital kartvizit.",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="tr" className="dark">
      <body className={inter.className}>{children}</body>
    </html>
  );
}
