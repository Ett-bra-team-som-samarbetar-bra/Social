import { z } from "zod";

export const descriptionSchema = z.object({
  newDescription: z.string().max(300),
});

export type DescriptionInput = z.infer<typeof descriptionSchema>;
