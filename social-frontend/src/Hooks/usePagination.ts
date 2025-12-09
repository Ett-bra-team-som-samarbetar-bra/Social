/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useState } from "react";

export function usePagination<T>(
  fetchPage: (page: number, size: number) => Promise<T[]>,
  pageSize: number,
  deps: any[] = []
) {
  const [page, setPage] = useState(1);
  const [items, setItems] = useState<T[]>([]);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setPage(1);
    setItems([]);
    setHasMore(true);
    setError(null);
  }, deps);

  useEffect(() => {
    let ignore = false;

    async function load() {
      setLoading(true);
      setError(null);

      try {
        const newItems = await fetchPage(page, pageSize);
        if (ignore) return;

        setItems((prev) => (page === 1 ? newItems : [...prev, ...newItems]));

        setHasMore(newItems.length === pageSize);
      } catch (err: any) {
        if (!ignore) setError(err.message ?? "Error loading items");
      }

      setLoading(false);
    }

    load();
    return () => {
      ignore = true;
    };
  }, [page, pageSize, ...deps]);

  return {
    items,
    loading,
    error,
    hasMore,
    loadMore: () => setPage((p) => p + 1),
  };
}
