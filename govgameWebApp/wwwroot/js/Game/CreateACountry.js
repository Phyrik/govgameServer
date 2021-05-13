var currentFlag = "default";
var flags = ["france", "germany", "india", "russia", "uk", "usa", "italy"];
function randomiseFlag() {
    var flag = flags[Math.floor(Math.random() * flags.length)];
    document.getElementById("flagImg").src = "/images/flags/" + flag + ".png";
    currentFlag = flag;
}
/*
function getCanvasClickPosition(canvas, event) {
    const rect = canvas.getBoundingClientRect()
    const x = event.clientX - rect.left
    const y = event.clientY - rect.top
    return [Math.round(x), Math.round(y)]
}

var lastClickLocation

const mapCanvas = document.getElementById("map-canvas")
mapCanvas.width = window.innerWidth - 200
mapCanvas.style.marginLeft = "-10px"
mapCanvas.style.marginRight = "-10px"
mapCanvas.height = window.innerWidth - 200
const ctx = mapCanvas.getContext("2d")
var regionMap = new Image()
regionMap.onload = function () {
    ctx.drawImage(regionMap, 0, 0, regionMap.width, regionMap.height, 0, 0, mapCanvas.width, mapCanvas.height)
}
regionMap.src = '/images/maps/world map.png'
mapCanvas.addEventListener("mousedown", function (e) {
    ctx.drawImage(regionMap, 0, 0, regionMap.width, regionMap.height, 0, 0, mapCanvas.width, mapCanvas.height)
    clickLocation = getCanvasClickPosition(mapCanvas, e)
    x = clickLocation[0]
    y = clickLocation[1]
    ctx.beginPath()
    ctx.fillStyle = "rgba(227, 178, 0, 0.5)"
    ctx.fillRect(x - (mapCanvas.width * (50 / 2000)), y - (mapCanvas.height * (50 / 2000)), (mapCanvas.width * (50 / 2000)) * 2, (mapCanvas.height * (50 / 2000)) * 2)
    ctx.stroke()

    mapClickLocation = [Math.round(x / mapCanvas.width * 2000), Math.round(y / mapCanvas.height * 2000)]

    console.log(mapClickLocation);

    lastClickLocation = mapClickLocation
})
*/
function formSubmitted() {
    var form = document.getElementById("country-form");
    var flagInput = document.createElement("input");
    flagInput.setAttribute("name", "flag-name");
    flagInput.setAttribute("type", "text");
    flagInput.style.display = "none";
    flagInput.setAttribute("value", currentFlag);
    form.appendChild(flagInput);
    /*
    var locationXInput = document.createElement("input")
    locationXInput.setAttribute("name", "locationX")
    locationXInput.setAttribute("type", "text")
    locationXInput.style.display = "none"
    locationXInput.setAttribute("value", lastClickLocation[0])
    form.appendChild(locationXInput)

    var locationYInput = document.createElement("input")
    locationYInput.setAttribute("name", "locationY")
    locationYInput.setAttribute("type", "text")
    locationYInput.style.display = "none"
    locationYInput.setAttribute("value", lastClickLocation[1])
    form.appendChild(locationYInput)
    */
    form.submit();
}
//# sourceMappingURL=CreateACountry.js.map