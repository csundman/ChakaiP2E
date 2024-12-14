import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import AppShell from "./AppShell";
import { AuthProvider } from '../lib/AuthContext';

const geistSans = localFont({
  src: "./fonts/GeistVF.woff",
  variable: "--font-geist-sans",
  weight: "100 900",
});
const geistMono = localFont({
  src: "./fonts/GeistMonoVF.woff",
  variable: "--font-geist-mono",
  weight: "100 900",
});

export const metadata: Metadata = {
  title: "Chakai P2E",
  description: "Pathfinder 2nd Edition character management tool.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <AuthProvider>
      <html lang="en" className="h-full">
        <body
          className={`${geistSans.variable} ${geistMono.variable} antialiased h-full`}
        >
          <AppShell>{children}</AppShell>
        </body>
      </html>
    </AuthProvider>
  );
}
