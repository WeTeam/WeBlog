function ToggleVisibility(elementId) {
    switch(document.getElementById(elementId).style.display)
    {
    case "block":
      document.getElementById(elementId).style.display = "none";
      break;    
    case "none":
      document.getElementById(elementId).style.display = "block";
      break;
    default:
        document.getElementById(elementId).style.display = "block";
    }
}