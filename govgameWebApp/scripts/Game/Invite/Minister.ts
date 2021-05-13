declare var isPrimeMinister: boolean;
declare var noMinistersToReplace: boolean;
declare var ministryCode: string;
declare var newCountryName: string;

function accept(ministerToReplacePM) {
    if (isPrimeMinister && !noMinistersToReplace) {
        if (ministerToReplacePM === null) {
            document.getElementById("pm-ministry-choice-div").style.display = ""
            return;
        }

        document.getElementById("please-wait-h4").style.display = ""

        $.post(`/Game/AcceptMinistryInvite?ministry=${ministryCode}&newCountryName=${newCountryName}&ministryToReplacePM=${ministerToReplacePM}`, function (data, status) {
            if (data == "success") {
                alert("Successfully accepted invite")
                location.href = "/Game/CountryDashboard"
            }
            else {
                alert(data)
            }
        })

        return;
    }

    if (noMinistersToReplace) {
        if (!confirm("Are you sure you want to leave your old country? It will be deleted if you choose to do so.")) {
            return
        }
    }

    document.getElementById("please-wait-h4").style.display = ""

    $.post(`/Game/AcceptMinistryInvite?ministry=${ministryCode}&newCountryName=${newCountryName}`, function (data, status) {
        if (data == "success") {
            alert("Successfully accepted invite")
            location.href = "/Game/CountryDashboard"
        }
    })
}

function decline() {
    document.getElementById("please-wait-h4").style.display = ""

    $.post(`/Game/DeclineMinistryInvite?ministry=${ministryCode}&newCountryName=${newCountryName}`, function (data, status) {
        if (data == "success") {
            alert("Successfully declined invite")
            location.href = "/Game/CountryDashboard"
        }
        else {
            alert(data)
        }
    })
}
