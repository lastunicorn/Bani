﻿// Bani
// Copyright (C) 2022 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Threading;
using System.Threading.Tasks;
using DustInTheWind.Bani.DataAccess.Port;
using DustInTheWind.Bani.Domain;
using DustInTheWind.Bani.Infrastructure;
using MediatR;

namespace DustInTheWind.Bani.Avalonia.Application.SelectIssuer;

internal class SelectIssuerUseCase : IRequestHandler<SelectIssuerRequest>
{
    private readonly ApplicationState applicationState;
    private readonly EventBus eventBus;
    private readonly IUnitOfWork unitOfWork;

    public SelectIssuerUseCase(ApplicationState applicationState, EventBus eventBus, IUnitOfWork unitOfWork)
    {
        this.applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public Task<Unit> Handle(SelectIssuerRequest request, CancellationToken cancellationToken)
    {
        Issuer issuer = unitOfWork.IssuerRepository.Get(request.IssuerId);

        applicationState.CurrentIssuer = request.IssuerId;

        RaiseIssuerChangedEvent(issuer);

        return Task.FromResult(Unit.Value);
    }

    private void RaiseIssuerChangedEvent(Issuer issuer)
    {
        IssuerChangedEvent ev = new()
        {
            Issuer = issuer
        };

        eventBus.Publish(ev);
    }
}