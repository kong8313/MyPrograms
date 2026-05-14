using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Confirmit.CATI.Core.Timezones;
using Microsoft.SqlServer.Management.Common;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators
{

    public class SchedulingObjectValidator : ISchedulingObjectValidator
    {
        private readonly ITimezoneManager _timezoneManager;

        public SchedulingObjectValidator(ITimezoneManager timezoneManager)
        {
            _timezoneManager = timezoneManager;
        }

        public bool Validate(CustomParameter customParameter, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!customParameter.Id.HasValue)
            {
                errors.Add(new Error(Strings.IdentifierNotInitializedMessage));
            }

            if (String.IsNullOrEmpty(customParameter.Name))
            {
                errors.Add(new Error(
                    String.Format(Strings.CustomParameterNameNotSpecifiedMessage, customParameter.Id)));
            }

            if (!customParameter.Type.HasValue)
            {
                errors.Add(new Error(
                    String.Format(Strings.CustomParameterTypeNotSpecifiedMessage, customParameter.Id)));
            }

            if (!customParameter.Value.HasValue)
            {
                errors.Add(new Error(
                    String.Format(Strings.CustomParameterValueNotSpecifiedMessage, customParameter.Id)));
            }

            return (errors.Count == 0);
        }

        public bool Validate(CustomScript script, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!script.Id.HasValue)
            {
                errors.Add(new Error(Strings.IdentifierNotInitializedMessage));
            }

            return (errors.Count == 0);
        }

        public bool Validate(ExclusionData data, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!data.StartDate.HasValue)
            {
                errors.Add(new Error(Strings.StartDateNotInitializedMessage));
            }

            if (!data.EndDate.HasValue)
            {
                errors.Add(new Error(Strings.EndDateNotInitializedMessage));
            }

            if (data.StartDate.HasValue && data.EndDate.HasValue && data.StartDate.Value > data.EndDate.Value)
            {
                errors.Add(new Error(Strings.StartDateGreaterEndDateMessage));
            }

            return (errors.Count == 0);
        }

        public bool Validate(Rule rule, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!rule.Id.HasValue)
            {
                errors.Add(new Error(Strings.IdentifierNotInitializedMessage));
            }

            return (errors.Count == 0);
        }

        public bool Validate(Schedule schedule, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!schedule.Id.HasValue)
            {
                errors.Add(new Error(Strings.IdentifierNotInitializedMessage));
            }

            return (errors.Count == 0);
        }

        public bool Validate(ShiftData data, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!data.StartDayOfWeek.HasValue || !data.StartTime.HasValue)
            {
                errors.Add(new Error(Strings.StartDateNotInitializedMessage));
            }

            if (!data.EndDayOfWeek.HasValue || !data.EndTime.HasValue)
            {
                errors.Add(new Error(Strings.EndDateNotInitializedMessage));
            }

            if (errors.Count == 0)
            {
                if (data.IsAcrossWeekend())
                {
                    errors.Add(new Error(Strings.StartDateGreaterEndDateMessage));
                }
            }

            return (errors.Count == 0);
        }

        public bool Validate(ShiftType shiftType, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!shiftType.Id.HasValue)
            {
                errors.Add(new Error(Strings.IdentifierNotInitializedMessage));
            }

            if (string.IsNullOrEmpty(shiftType.Name))
            {
                errors.Add(new Error(Strings.NameNotInitializedMessage));
            }

            if (!shiftType.Color.HasValue)
            {
                errors.Add(new Error(Strings.ColorNotInitializedMessage));
            }

            return (errors.Count == 0);
        }

        public bool Validate(SubRuleAction entity, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!entity.Id.HasValue)
            {
                errors.Add(new Error(Strings.IdentifierNotInitializedMessage));
            }

            if (!entity.ActionId.HasValue)
            {
                errors.Add(new Error(Strings.ActionNotInitializedMessage));
            }
            else
            {
                var scheduleService = ServiceLocator.Resolve<Confirmit.CATI.Core.Services.IScheduleService>();
                Confirmit.CATI.Core.ScheduleDom.Script.Action action = scheduleService.GetActions().GetActionById(entity.ActionId.Value);
                if (action == null)
                {
                    errors.Add(
                        new Error(
                            String.Format(Strings.UnrecognizedActionMessage, entity.ActionId.Value)
                            )
                        );
                }
                else if (action.HasParameter)
                {
                    if (entity.Parameter.Type == Parameter.ParamType.Constant)
                    {
                        if (String.IsNullOrEmpty(entity.Parameter.Constant))
                        {
                            errors.Add(
                                new Error(
                                    String.Format(Strings.ActionParameterNotSpecifiedMessage, action.Name)
                                    )
                                );
                        }
                        else
                        {
                            // we are trying to convert specified parameter to type of parameter
                            // which is described in action dictionary.

                            Type parameterType = null;

                            try
                            {
                                parameterType = Type.GetType(action.ParameterTypeName);
                            }
                            catch (ArgumentNullException /*ex*/)
                            {
                            }
                            catch (TargetInvocationException /*ex*/)
                            {
                            }
                            catch (ArgumentException /*ex*/)
                            {
                            }
                            catch (TypeLoadException /*ex*/)
                            {
                            }

                            if (parameterType == null)
                            {
                                errors.Add(
                                    new Error(
                                        String.Format(
                                            Strings.ActionParameterTypeNotSpecifiedMessage,
                                            action.Name
                                            )
                                        )
                                    );
                            }
                            else
                            {
                                if (!SchedulingUtilities.CheckStringValueOfType(entity.Parameter.Constant, parameterType))
                                {
                                    errors.Add(
                                        new Error(
                                            String.Format(
                                                Strings.ParameterValueCouldntBeConvertedMessage,
                                                entity.Parameter.Constant
                                                )
                                            )
                                        );
                                }
                            }
                        }
                    }
                }
            }

            return (errors.Count == 0);
        }

        public bool Validate(SubRule subRule, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (!subRule.Id.HasValue)
            {
                errors.Add(new Error(Strings.IdentifierNotInitializedMessage));
            }

            return (errors.Count == 0);
        }

        public bool Validate<T>(T item, out ErrorCollection errors)
        {
            errors = new ErrorCollection();
            var type = typeof(T);

            if (type == typeof(ShiftType))
            {
                return Validate(item as ShiftType, out errors);
            }

            if (type == typeof(CustomParameter))
            {
                return Validate(item as CustomParameter, out errors);
            }

            if (type == typeof(CustomScript))
            {
                return Validate(item as CustomScript, out errors);
            }

            if (type == typeof(ShiftData))
            {
                return Validate((ShiftData)(object)item, out errors);
            }

            if (type == typeof(ExclusionData))
            {
                return Validate((ExclusionData)(object)item, out errors);
            }

            if (type == typeof(Shift))
            {
                return true;
            }

            if (type == typeof(Exclusion))
            {
                return true;
            }

            if (type == typeof(Rule))
            {
                return Validate(item as Rule, out errors);
            }

            if (type == typeof(Schedule))
            {
                return Validate(item as Schedule, out errors);
            }

            if (type == typeof(SubRuleAction))
            {
                return Validate(item as SubRuleAction, out errors);
            }

            if (type == typeof(SubRule))
            {
                return Validate(item as SubRule, out errors);
            }

            throw new InvalidArgumentException("Unsupported type: " + type);
        }

        public bool ValidateWithCollection<T, TType>(BaseCollection<T, TType> baseCollection, T item, out ErrorCollection errors)
            where T : BaseObject<TType>
            where TType : struct
        {
            var type = typeof(T);

            if (type == typeof(CustomParameter))
            {
                return CheckParameter(item as CustomParameter, baseCollection as CustomParameterCollection, out errors);
            }

            if (type == typeof(ShiftType))
            {
                return CheckShiftType(item as ShiftType, baseCollection as ShiftTypeCollection, out errors);
            }

            if (type == typeof(Shift))
            {
                return CheckShift(item as Shift, baseCollection as ShiftCollection, out errors);
            }

            if (type == typeof(Exclusion))
            {
                return CheckShift(item as Exclusion, baseCollection as ExclusionCollection, out errors);
            }

            errors = new ErrorCollection();
            return true;
        }

        private bool CheckShift<TShift, TShiftData>(
            BaseShift<TShiftData> shift,
            BaseShiftCollection<TShift, TShiftData> collection,
            out ErrorCollection errors) where TShift : BaseShift<TShiftData>
		where TShiftData : IIntersectable<TShiftData>
        {
            errors = new ErrorCollection();

            if (shift == null)
            {
                errors.Add(new Error(Strings.ItemNullExceptionMessage));
            }
            else
            {
                if (!shift.Id.HasValue)
                {
                    errors.Add(new Error(Strings.IdentifierNotInitializedMessage));
                }

                if (!shift.ShiftTypeId.HasValue)
                {
                    errors.Add(new Error(Strings.ShiftTypeNotInitializedMessage));
                }
            }

            if (errors.Count == 0)
            {
                int[] shiftTimezoneIds = shift.GetTimezoneIds();

                foreach (BaseShift<TShiftData> item in collection)
                {
                    if (shift.Id.Value == item.Id.Value)
                    {
                        // do not process the same shift
                        continue;
                    }

                    foreach (int timezoneId in
                        SchedulingUtilities.Combine<int>(item.GetTimezoneIds(), shiftTimezoneIds))
                    {
                        TShiftData shiftData;
                        TShiftData itemData;
                        if (shift.TryGetDataForTimezone(timezoneId, out shiftData) &&
                            item.TryGetDataForTimezone(timezoneId, out itemData))
                        {
                            if (shiftData.HasIntersection(itemData))
                            {
                                BvTimezoneEntity timeZone;
                                bool existTimeZone = _timezoneManager.TimezonesList.TryGetItemById(timezoneId, out timeZone);
                                String timeZoneInfo;
                                if (existTimeZone)
                                    timeZoneInfo = timeZone.Name;
                                else
                                {
                                    timeZoneInfo = timezoneId.ToString();
                                    Trace.TraceInformation(
                                                String.Format(
                                                            Strings.CantGetTimeZoneMessage,
                                                            timezoneId
                                                )
                                    );
                                }
                                errors.Add(new Error(
                                    String.Format(
                                        Strings.ShiftIntersectionMessage,
                                        item.Id.Value,
                                        timezoneId == Shift.RespondentTimezoneId ?
                                            Strings.RespondentTimezoneName : timeZoneInfo
                                        )
                                    )
                                );
                            }
                        }
                    }
                }
            }

            return (errors.Count == 0);
        }

        private bool CheckShiftType(ShiftType shiftType, ShiftTypeCollection collection, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (shiftType == null)
            {
                errors.Add(new Error(Strings.ItemNullExceptionMessage));
            }
            else
            {
                bool isNameInUse = collection.Any(x => (x.Id != shiftType.Id && x.Name == shiftType.Name));

                if (isNameInUse)
                {
                    errors.Add(new Error(Strings.ShiftTypeWithTheSameNameAlreadyExists));
                }
            }

            return (errors.Count == 0);
        }

        private bool CheckParameter(CustomParameter param, CustomParameterCollection collection, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (param == null)
            {
                errors.Add(new Error(Strings.ItemNullExceptionMessage));
            }
            else
            {
                bool isNameInUse = collection.Any(x => (x.Id != param.Id && x.Name == param.Name));

                if (isNameInUse)
                {
                    errors.Add(new Error(Strings.ParameterWithTheSameNameAlreadyExists));
                }
            }

            return (errors.Count == 0);
        }
    }
}