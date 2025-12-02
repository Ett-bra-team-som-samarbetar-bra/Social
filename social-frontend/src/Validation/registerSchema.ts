import { z } from "zod";

export const registerSchema = z.object({
  username: z
    .string()
    .nonempty("Username must not be empty")
    .min(3, "Username must be at least 3 characters")
    .max(50, "Username cannot contain more than 50 characters"),
  email: z.email().max(50),
  password: z
    .string()
    .nonempty("Password must not be empty")
    .min(6, "Password must be at least 6 characters")
    .regex(/[A-Z]/, "Password must contain at least one uppercase letter.")
    .regex(/[a-z]/, "Password must contain at least one lowercase letter.")
    .regex(/[0-9]/, "Password must contain at least one number.")
    .regex(
      /[^a-zA-Z0-9]/,
      "Password must contain at least one special character."
    )
    .regex(/^\S+$/, "Password cannot contain whitespace."),
  description: z.string().max(300),
});

export type RegisterInput = z.infer<typeof registerSchema>;
