export const useColorGenerator = () => {
  const generateColor = (value: number) => {
    // Ensure the value is within the valid range (0-100)
    value = Math.min(100, Math.max(0, value));

    // Map the value to a hue between 0 (red) and 120 (green)
    const hue = (value / 100) * 120;
    // Set saturation and lightness to fixed values
    const saturation = 100;
    const lightness = 50;

    // Convert HSL to RGB
    const rgb = hslToRgb(hue, saturation, lightness);

    // Convert RGB values to hex
    const colorHex = rgbToHex(rgb[0], rgb[1], rgb[2]);

    return colorHex;
  };

  const hslToRgb = (h: number, s: number, l: number) => {
    h /= 360;
    s /= 100;
    l /= 100;
    let r, g, b;

    if (s === 0) {
      r = g = b = l; // achromatic
    } else {
      const hue2rgb = function hue2rgb(p: number, q: number, t: number) {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1 / 6) return p + (q - p) * 6 * t;
        if (t < 1 / 2) return q;
        if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
        return p;
      };

      const q = l < 0.5 ? l * (1 + s) : l + s - l * s;
      const p = 2 * l - q;

      r = hue2rgb(p, q, h + 1 / 3);
      g = hue2rgb(p, q, h);
      b = hue2rgb(p, q, h - 1 / 3);
    }

    return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
  };

  const rgbToHex = (r: number, g: number, b: number) => {
    return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
  };

  const componentToHex = (c: number) => {
    const hex = c.toString(16);
    return hex.length == 1 ? "0" + hex : hex;
  };

  const hexToRgb = (hex: string) => {
    const bigint = parseInt(hex.slice(1), 16);
    return {
      r: (bigint >> 16) & 255,
      g: (bigint >> 8) & 255,
      b: bigint & 255,
    };
  };

  return {
    generateColor,
    hslToRgb,
    rgbToHex,
    hexToRgb,
  };
};
