<template>
  <div class="vacation-form">
    <div>
      <label for="language">Language:</label>
      <select id="language" v-model="selectedLanguage" @change="onLanguageChange">
        <option value="en">🇬🇧 English</option>
        <option value="no">🇳🇴 Norsk</option>
        <option value="sv">🇸🇪 Svenska</option>
      </select>
    </div>

    <div v-if="employees.length > 0">
      <h3>{{ $t('selectEmployee') }}</h3>
      <select v-model="selectedEmployee" @change="onEmployeeChange">
        <option disabled value="">{{ $t('pleaseSelect') }}</option>
        <option v-for="e in employees" :key="e.id" :value="e">{{ e.name }}</option>
      </select>
    </div>

    <div v-if="selectedEmployee">
      <h2>{{ $t('registerVacationFor') }} {{ selectedEmployee.name }}</h2>

      <div class="warning-box">
        <span class="warning-icon">❗</span>
        <span>{{ $t('publicHolidayNote') }}</span>
      </div>

      <label>
        {{ $t('startDate') }}:
        <Datepicker v-model="startDate" :highlight="highlightedDates" :disabled-dates="disabledDates"
          :enable-time-picker="false" :max-date="endDate" :year-range="[currentYear, currentYear]"
          @update:modelValue="onStartDateChange" />
      </label>

      <label>
        {{ $t('endDate') }}:
        <Datepicker v-model="endDate" :highlight="highlightedDates" :disabled-dates="disabledDates"
          :enable-time-picker="false" :min-date="startDate" :year-range="[currentYear, currentYear]" />
      </label>

      <label>
        {{ $t('note') }}:
        <textarea v-model="note"></textarea>
      </label>

      <button @click="submitRequest">{{ $t('registerVacation') }}</button>

      <!--       <div class="calendar-info">
        <p><strong>{{ $t('publicHolidays') }} {{ currentYear }}:</strong></p>
        <ul>
          <li v-for="d in publicVacationDays" :key="d">{{ d }}</li>
        </ul>
      </div> -->

      <div v-if="error" class="error">{{ error }}</div>
      <div v-if="success" class="success">{{ success }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import Datepicker from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css';

const { locale, t } = useI18n();
const selectedLanguage = ref(localStorage.getItem('language') || 'en');
locale.value = selectedLanguage.value;

const selectedEmployee = ref('');
const employees = ref([]);
const publicVacationDays = ref([]);
const vacationBalance = ref(null);
const startDate = ref(null);
const endDate = ref(null);
const note = ref('');
const error = ref('');
const success = ref('');
const currentYear = new Date().getFullYear();

const highlightedDates = computed(() => ({
  dates: publicVacationDays.value.map((d) => new Date(d)),
  includeDisabled: true
}));

const disabledDates = (date) => date.getFullYear() !== currentYear;

const onLanguageChange = () => {
  locale.value = selectedLanguage.value;
  localStorage.setItem('language', selectedLanguage.value);
};

const fetchEmployees = async () => {
  try {
    const response = await fetch('/employee', {
      headers: {
        'Accept-Language': selectedLanguage.value
      }
    });
    employees.value = await response.json();
  } catch {
    error.value = t('errorFetchEmployees');
  }
};

const onEmployeeChange = async () => {
  try {
    const employee = selectedEmployee.value;
    const year = currentYear;

    const vacationDaysResponse = await fetch(`/company/${employee.companyId}/vacation-days/${year}`, {
      headers: {
        'Accept-Language': selectedLanguage.value
      }
    });
    publicVacationDays.value = await vacationDaysResponse.json();

    const balanceResponse = await fetch(`/employee/${employee.id}/vacation-balance/${year}`, {
      headers: {
        'Accept-Language': selectedLanguage.value
      }
    });
    vacationBalance.value = await balanceResponse.json();

    startDate.value = null;
    endDate.value = null;
    note.value = '';
    error.value = '';
    success.value = '';
  } catch {
    error.value = t('errorFetchEmployeeData');
  }
};

const onStartDateChange = (date) => {
  startDate.value = date;
  if (endDate.value && new Date(endDate.value) < new Date(date)) {
    endDate.value = null;
  }
};

const submitRequest = async () => {
  error.value = '';
  success.value = '';

  if (!startDate.value || !endDate.value) {
    error.value = t('errorSelectDates');
    return;
  }

  const start = new Date(startDate.value);
  const end = new Date(endDate.value);

  if (start.getFullYear() !== currentYear || end.getFullYear() !== currentYear) {
    error.value = t('errorInvalidYear');
    return;
  }

  if (start > end) {
    error.value = t('errorStartAfterEnd');
    return;
  }

  const dto = {
    employeeId: selectedEmployee.value.id,
    startDate: start.toISOString().split('T')[0],
    endDate: end.toISOString().split('T')[0],
    note: note.value,
    approverAdminId: 1 // Hard coded. This would generally use the logged in user.
  };

  try {
    const response = await fetch(`/VacationRequest`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept-Language': selectedLanguage.value
      },
      body: JSON.stringify(dto)
    });

    if (!response.ok) {
      const result = await response.json();
      error.value = result?.value || t('errorSubmitRequest');
      return;
    }

    success.value = t('successRequest');
  } catch {
    error.value = t('errorSubmitRequest');
  }
};

onMounted(fetchEmployees);
</script>

<style scoped>
.vacation-form {
  margin: 20px;
  max-width: 600px;
  padding: 20px;
  border: 1px solid #ccc;
  border-radius: 8px;
  background-color: #fafafa;
}

label {
  display: block;
  margin-top: 12px;
}

input[type="date"],
textarea,
select {
  display: block;
  width: 100%;
  margin-top: 4px;
  margin-bottom: 8px;
  padding: 8px;
  border-radius: 4px;
  border: 1px solid #aaa;
}

button {
  margin-top: 12px;
  padding: 10px 20px;
  background-color: #2c3e50;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #1a252f;
}

.error {
  margin-top: 12px;
  color: red;
}

.success {
  margin-top: 12px;
  color: green;
}

.warning-box {
  display: flex;
  align-items: center;
  background-color: #fff3cd;
  border: 1px solid #ffeeba;
  padding: 10px;
  margin: 12px 0;
  border-radius: 5px;
  color: #856404;
  font-size: 0.95em;
}

.warning-icon {
  font-size: 1.5em;
  margin-right: 10px;
  flex-shrink: 0;
}
</style>
