import * as React from "react";
import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";

const badgeVariants = cva(
  "inline-flex items-center gap-2 rounded-full px-3 py-1 text-sm font-medium transition-colors",
  {
    variants: {
      variant: {
        // Endorse edilmiş skill — mor.
        endorsed: "bg-primary/15 text-primary border border-primary/30",
        // Endorse edilmemiş / nötr skill.
        neutral: "bg-muted text-foreground border border-border",
      },
    },
    defaultVariants: {
      variant: "neutral",
    },
  }
);

export interface BadgeProps
  extends React.HTMLAttributes<HTMLSpanElement>,
    VariantProps<typeof badgeVariants> {}

function Badge({ className, variant, ...props }: BadgeProps) {
  return <span className={cn(badgeVariants({ variant }), className)} {...props} />;
}

export { Badge, badgeVariants };
