export function toDateLong(date: string, locale = "en-GB") {
  return new Date(date).toLocaleString(locale, {
    hour: "2-digit",
    minute: "2-digit",
    day: "numeric",
    month: "short",
    year: "numeric",
  });
}
