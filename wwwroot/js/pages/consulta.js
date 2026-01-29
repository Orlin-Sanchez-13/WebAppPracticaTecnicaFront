import { ParticipantesTable } from '../modules/participantesTable.js';

(function() {
    const participantesData = window.participantesData || [];
    const participantesTable = new ParticipantesTable(participantesData);
    participantesTable.init();
})();