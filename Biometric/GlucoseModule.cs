﻿using System;
using System.Collections.Generic;
using AutoMapper;
using Nancy;
using Nancy.ModelBinding;
using Toffees.Glucose.Data.Repositories;
using Toffees.Glucose.Models.DTOs;

namespace Toffees.Glucose
{
    public sealed class GlucoseModule : NancyModule
    {
        public GlucoseModule(IGlucoseRepository glucosesRepository) : base("/api/biometric/glucose")
        {
            Get("/{userid}", async parameters =>
            {
                var startDateTime = Request.Query["startDateTime"];
                if (startDateTime.Value == null)
                {
                    var allBloodSamples = Mapper.Map<List<GlucoseDto>>(await glucosesRepository.GetAllGlucosesTaskAsync(parameters.userid));
                    if (allBloodSamples == null || allBloodSamples.Count == 0)
                    {
                        return HttpStatusCode.NoContent;
                    }
                    return allBloodSamples;
                }
                var listOfBloodSamples = Mapper.Map<List<GlucoseDto>>(await glucosesRepository.GetGlucosesByDateTimeSpanTaskAsync(parameters.userid, DateTime.Parse(startDateTime), DateTime.Now.ToUniversalTime().AddHours(3)));
                if (listOfBloodSamples == null || listOfBloodSamples.Count == 0)
                {
                    return HttpStatusCode.NoContent;
                }
                return Mapper.Map<List<GlucoseDto>>(await glucosesRepository.GetAllGlucosesTaskAsync(parameters.userid));
            });

            Post("/{userid}", async (parameters, _) =>
            {
                var glucoseDto = this.BindAndValidate<GlucoseDto>();
                if (!ModelValidationResult.IsValid)
                {
                    return Negotiate.WithModel(ModelValidationResult).WithStatusCode(HttpStatusCode.BadRequest);
                }
                var newGlucose = Mapper.Map<Data.Entities.Glucose>(glucoseDto);
                newGlucose.UserId = parameters.userid;
                glucosesRepository.InsertGlucose(newGlucose);
                await glucosesRepository.SaveAsync().ConfigureAwait(false);
                return newGlucose;
            });

            Put("/{userId}", async (parameters, _) => 
            {
                var glucoseDto = this.BindAndValidate<GlucoseDto>();
                if (!ModelValidationResult.IsValid)
                {
                    return Negotiate.WithModel(ModelValidationResult).WithStatusCode(HttpStatusCode.BadRequest);
                }
                var glucose = await glucosesRepository.GetGlucosesByIdTaskAsync(glucoseDto.Id).ConfigureAwait(false);
                glucose.Tag = glucoseDto.Tag;
                glucose.Data = glucoseDto.Data;
                glucosesRepository.UpdateGlucose(glucose);
                await glucosesRepository.SaveAsync().ConfigureAwait(false);
                return Mapper.Map<GlucoseDto>(glucose);
            });

            Delete("/{gid}", async parameters =>
            {
                var glucose = await glucosesRepository.GetGlucosesByIdTaskAsync(int.Parse(parameters.gid)).ConfigureAwait(false);
                glucosesRepository.DeleteGlucose(glucose.Id);
                await glucosesRepository.SaveAsync().ConfigureAwait(false);
                return HttpStatusCode.OK;
            });
        }
    }
}
