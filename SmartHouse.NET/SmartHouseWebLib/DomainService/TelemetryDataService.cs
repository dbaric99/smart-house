﻿using SmartHouseWebLib.DomainService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouseWebLib.Models;
using SmartHouseWebLib.UnitOfWork;

namespace SmartHouseWebLib.DomainService
{
    public class TelemetryDataService : ITelemetryDataService
    {
        private readonly IUnitOfWork unitOfWork;

        public TelemetryDataService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<int> Delete(TelemetryData model)
        {
            unitOfWork.TelemetryDataRepository.Delete(model);
            return await unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<TelemetryData>> GetAllAsync()
        {
            return await unitOfWork.TelemetryDataRepository.GetAsync();
        }

        public async Task<TelemetryData> GetAsync(int Id)
        {
            return await unitOfWork.TelemetryDataRepository.GetByIDAsync(Id);
        }

        public async Task<int> Insert(TelemetryData model)
        {
            unitOfWork.TelemetryDataRepository.Insert(model);
            return await unitOfWork.SaveAsync();
        }

        public async Task<int> Update(TelemetryData model)
        {
            unitOfWork.TelemetryDataRepository.Update(model);
            return await unitOfWork.SaveAsync();
        }
    }
}