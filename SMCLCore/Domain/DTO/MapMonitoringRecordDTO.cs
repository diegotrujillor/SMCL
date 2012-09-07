using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMCLCore.Domain.DTO
{
    public class MapMonitoringRecordDTO
    {
        private string appId;
        private string sigId;
        private string monValue;
        private string sigMeasure;

        public MapMonitoringRecordDTO() { }

        public MapMonitoringRecordDTO(string appId, string sigId, string monValue, string sigMeasure)
        {
            this.appId = appId;
            this.sigId = sigId;
            this.monValue = monValue;
            this.sigMeasure = sigMeasure;
        }

        public string AppId
        {
            get { return this.appId; }
            set { this.appId = value; }
        }

        public string SigId
        {
            get { return this.sigId; }
            set { this.sigId = value; }
        }

        public string MonValue
        {
            get { return this.monValue; }
            set { this.monValue = value; }
        }

        public string SigMeasure
        {
            get { return this.sigMeasure; }
            set { this.sigMeasure = value; }
        }
    }
}