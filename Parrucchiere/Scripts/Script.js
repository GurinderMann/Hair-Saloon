const calendar = document.querySelector(".calendar"),
    date = document.querySelector(".date"),
    daysContainer = document.querySelector(".days"),
    prev = document.querySelector(".prev"),
    next = document.querySelector(".next"),
    todayBtn = document.querySelector(".today-btn"),
    gotoBtn = document.querySelector(".goto-btn"),
    dateInput = document.querySelector(".date-input"),
    eventDay = document.querySelector(".event-day"),
    eventDate = document.querySelector(".event-date"),
    eventsContainer = document.querySelector(".events"),
    addEventBtn = document.querySelector(".add-event"),
    addEventWrapper = document.querySelector(".add-event-wrapper "),
    addEventCloseBtn = document.querySelector(".close "),
    addEventTitle = document.querySelector(".event-name "),
    addEventFrom = document.querySelector(".event-time-from "),
    addEventTo = document.querySelector(".event-time-to "),
    addEventSubmit = document.querySelector(".add-event-btn ");

let today = new Date();
let activeDay;
let month = today.getMonth();
let year = today.getFullYear();

const months = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
];

const eventsArr = [];

// Function to add days in days with class day and prev-date next-date on previous month and next month days and active on today
function initCalendar() {
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const prevLastDay = new Date(year, month, 0);
    const prevDays = prevLastDay.getDate();
    const lastDate = lastDay.getDate();
    const day = firstDay.getDay();
    const nextDays = 7 - lastDay.getDay() - 1;

    date.innerHTML = months[month] + " " + year;

    let days = "";

    for (let x = day; x > 0; x--) {
        days += `<div class="day prev-date">${prevDays - x + 1}</div>`;
    }

    for (let i = 1; i <= lastDate; i++) {
        if (
            i === new Date().getDate() &&
            year === new Date().getFullYear() &&
            month === new Date().getMonth()
        ) {
            activeDay = i;
            getActiveDay(i);
            if (event) {
                days += `<div class="day today active">${i}</div>`;
            } else {
                days += `<div class="day today active">${i}</div>`;
            }
        } else {
            days += `<div class="day">${i}</div>`;
        }
    }

    for (let j = 1; j <= nextDays; j++) {
        days += `<div class="day next-date">${j}</div>`;
    }
    daysContainer.innerHTML = days;
    addListener();
}

// Function to add month and year on prev and next button
function prevMonth() {
    month--;
    if (month < 0) {
        month = 11;
        year--;
    }
    initCalendar();
}

function nextMonth() {
    month++;
    if (month > 11) {
        month = 0;
        year++;
    }
    initCalendar();
}

prev.addEventListener("click", prevMonth);
next.addEventListener("click", nextMonth);

initCalendar();

// Function to add active on day
function addListener() {
    const days = document.querySelectorAll(".day");
    days.forEach((day) => {
        day.addEventListener("click", (e) => {
            getActiveDay(e.target.innerHTML);
            activeDay = Number(e.target.innerHTML);
            // Remove active
            days.forEach((day) => {
                day.classList.remove("active");
            });
            // If clicked prev-date or next-date switch to that month
            if (e.target.classList.contains("prev-date")) {
                prevMonth();
                // Add active to clicked day after month is changed
                setTimeout(() => {
                    // Add active where no prev-date or next-date
                    const days = document.querySelectorAll(".day");
                    days.forEach((day) => {
                        if (
                            !day.classList.contains("prev-date") &&
                            day.innerHTML === e.target.innerHTML
                        ) {
                            day.classList.add("active");
                        }
                    });
                }, 100);
            } else if (e.target.classList.contains("next-date")) {
                nextMonth();
                // Add active to clicked day after month is changed
                setTimeout(() => {
                    const days = document.querySelectorAll(".day");
                    days.forEach((day) => {
                        if (
                            !day.classList.contains("next-date") &&
                            day.innerHTML === e.target.innerHTML
                        ) {
                            day.classList.add("active");
                        }
                    });
                }, 100);
            } else {
                e.target.classList.add("active");
            }
        });
    });
}

todayBtn.addEventListener("click", () => {
    today = new Date();
    month = today.getMonth();
    year = today.getFullYear();
    initCalendar();
});

dateInput.addEventListener("input", (e) => {
    dateInput.value = dateInput.value.replace(/[^0-9/]/g, "");
    if (dateInput.value.length === 2) {
        dateInput.value += "/";
    }
    if (dateInput.value.length > 7) {
        dateInput.value = dateInput.value.slice(0, 7);
    }
    if (e.inputType === "deleteContentBackward") {
        if (dateInput.value.length === 3) {
            dateInput.value = dateInput.value.slice(0, 2);
        }
    }
});

gotoBtn.addEventListener("click", gotoDate);

function gotoDate() {
    const dateArr = dateInput.value.split("/");
    if (dateArr.length === 2) {
        if (dateArr[0] > 0 && dateArr[0] < 13 && dateArr[1].length === 4) {
            month = dateArr[0] - 1;
            year = dateArr[1];
            initCalendar();
            return;
        }
    }
    alert("Invalid Date");
}

function getActiveDay(date) {
    const day = new Date(year, month, date);
    const dayName = day.toString().split(" ")[0];
    eventDay.innerHTML = dayName;
    const isMonday = day.getDay() === 1;
    const par = document.getElementById("Chiusura")
    eventDay.innerHTML = dayName;
    eventDate.innerHTML = date + " " + months[month] + " " + year;
    if (isMonday) {
        document.getElementById("crea").style.display = "none";

        par.innerHTML = "Siamo chiusi ";
        par.style.display = "block";
    } else {
        document.getElementById("crea").style.display = "block";
  
        par.style.display = "none";
    }

    // Get the selected time from the dropdown
    const selectedTime = document.getElementById("Ore").value;

    // Format the date and time as "yyyy-MM-ddThh:mm" (without seconds)
    const formattedDateTime = `${year}-${(month + 1).toString().padStart(2, '0')}-${date.toString().padStart(2, '0')}T${selectedTime}`;

    // Insert the formatted date and time into the input field with ID "Data"
    document.getElementById("Data").value = formattedDateTime;
   
}

document.getElementById("Ore").addEventListener("change", function () {
    updateSelectedDateTime();
});

// Funzione per aggiornare la data e l'ora selezionate
function updateSelectedDateTime() {
    // Ottieni il valore selezionato nel dropdown "Ore"
    const selectedTime = document.getElementById("Ore").value;

    // Ottieni la data corrente selezionata (dal calendario)
    const selectedDate = document.getElementById("Data").value;

    // Combina la data e l'ora selezionate in un formato compatibile con "datetime-local"
    const combinedDateTime = selectedDate.split('T')[0] + 'T' + selectedTime;

    // Imposta il valore combinato nella casella di input "Data"
    document.getElementById("Data").value = combinedDateTime;
}
