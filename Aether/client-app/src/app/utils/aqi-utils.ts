export function getAqiStatus(aqi: number) {
  if (aqi <= 50)
    return {
      label: "Good",
      lightBgColour: "bg-emerald-100",
      bgColour: "bg-emerald-500",
      borderColour: "border-emerald-400",
      textColour: "text-emerald-400",
      description:
        "Air quality is excellent. It's a great day for outdoor activities.",
    };
  if (aqi <= 100)
    return {
      label: "Moderate",
      lightBgColour: "bg-amber-100",
      bgColour: "bg-amber-500",
      borderColour: "border-amber-400",
      textColour: "text-amber-400",
      description:
        "Air quality is acceptable. However, there may be a risk for some people, particularly those who are unusually sensitive to air pollution",
    };
  if (aqi <= 150)
    return {
      label: "Unhealthy for Sensitive Groups",
      lightBgColour: "bg-orange-100",
      bgColour: "bg-orange-500",
      borderColour: "border-orange-400",
      textColour: "text-orange-400",
      description:
        "Members of sensitive groups may experience health effects. The general public is less likely to be affected",
    };
  if (aqi <= 200)
    return {
      label: "Unhealthy",
      lightBgColour: "bg-red-100",
      bgColour: "bg-red-500",
      borderColour: "border-red-400",
      textColour: "text-red-400",
      description:
        "Some members of the general public may experience health effects; members of sensitive groups may experience more serious health effects",
    };
  if (aqi <= 300)
    return {
      label: "Very Unhealthy",
      lightBgColour: "bg-purple-100",
      bgColour: "bg-purple-500",
      borderColour: "border-purple-400",
      textColour: "text-purple-400",
      description:
        "Health alert: everyone is likely to experience serious health effects",
    };
  return {
    label: "Hazardous",
    lightBgColour: "bg-maroon-100",
    bgColour: "bg-maroon-500",
    borderColour: "border-maroon-400",
    textColour: "text-maroon-400",
    description:
      "Health warning of emergency conditions: everyone may experience severe health effects",
  };
}
